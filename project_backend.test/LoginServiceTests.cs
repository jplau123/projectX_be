using Moq;
using project_backend.DTOs.RequestDTO;
using project_backend.Interfaces;
using project_backend.Model.Entities;
using project_backend.Services;

public class LoginServiceTests
{
    private readonly Mock<IUserAuthRepository> _mockUserAuthRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly LoginService _loginService;

    public LoginServiceTests()
    {
        _mockUserAuthRepo = new Mock<IUserAuthRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockAuthService = new Mock<IAuthService>();
        _loginService = new LoginService(_mockUserAuthRepo.Object, _mockUserRepo.Object, _mockAuthService.Object);
    }

    [Fact]
    public async Task Authenticate_ShouldReturnTokenResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var authRequest = new UserAuthRequest { UserName = "testUser", Password = "testPass" };

        // Use a valid BCrypt hash of "testPass" or any password you choose
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("testPass");
        var userAuth = new UserAuth { User_Name = "testUser", Password = hashedPassword, Active = true };

        var accessToken = "access_token";
        var refreshToken = "refresh_token";

        _mockAuthService.Setup(x => x.GetUserAuthDetails(authRequest.UserName)).ReturnsAsync(userAuth);
        _mockAuthService.Setup(x => x.SetupAccessToken(It.IsAny<UserAuth>())).ReturnsAsync(accessToken);
        _mockAuthService.Setup(x => x.SetupRefreshToken(It.IsAny<UserAuth>())).ReturnsAsync(refreshToken);

        // Act
        var result = await _loginService.Authenticate(authRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessToken, result.AccessToken);
        Assert.Equal(refreshToken, result.RefreshToken);
    }
    [Fact]
    public async Task Register_ShouldReturnUser_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var newUserRequest = new NewUserRequest
        {
            UserName = "newUser",
            Password = "newPass",
            ConfirmPassword = "newPass" // Ensure ConfirmPassword matches Password
        };
        var userId = 1;
        var user = new User { /* Initialize properties */ };

        _mockUserAuthRepo.Setup(x => x.UsernameExists(newUserRequest.UserName)).ReturnsAsync(false);
        _mockUserAuthRepo.Setup(x => x.SaveUser(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(userId);
        _mockUserRepo.Setup(x => x.GetUserById(userId)).ReturnsAsync(user);

        // Act
        var result = await _loginService.Register(newUserRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result);
    }
    [Fact]
    public async Task RefreshAccess_ShouldReturnNewTokenResponse_WhenTokensAreValid()
    {
        // Arrange
        var tokenRequest = new TokenRequest { AccessToken = "old_access_token", RefreshToken = "old_refresh_token" };
        var userAuth = new UserAuth { /* Initialize properties */ };
        var newAccessToken = "new_access_token";
        var newRefreshToken = "new_refresh_token";

        _mockAuthService.Setup(x => x.GetUserFromToken(tokenRequest.AccessToken)).ReturnsAsync(userAuth);
        _mockAuthService.Setup(x => x.IsTokenValid(userAuth, tokenRequest.RefreshToken)).Returns(true);
        _mockAuthService.Setup(x => x.SetupAccessToken(userAuth)).ReturnsAsync(newAccessToken);
        _mockAuthService.Setup(x => x.SetupRefreshToken(userAuth)).ReturnsAsync(newRefreshToken);

        // Act
        var result = await _loginService.RefreshAccess(tokenRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newAccessToken, result.AccessToken);
        Assert.Equal(newRefreshToken, result.RefreshToken);
    }
    [Fact]
    public async Task RevokeAccess_ShouldInvokeRevokeAndDeleteMethods()
    {
        // Arrange
        _mockAuthService.Setup(x => x.RevokeRefreshToken()).Returns(Task.CompletedTask);

        // Act
        await _loginService.RevokeAccess();

        // Assert
        _mockAuthService.Verify(x => x.RevokeRefreshToken(), Times.Once);
        _mockAuthService.Verify(x => x.DeleteAccessTokenCookie(), Times.Once);
        _mockAuthService.Verify(x => x.DeleteRefreshTokenCookie(), Times.Once);
    }
}
