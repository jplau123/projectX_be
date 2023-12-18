
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using project_backend.Helpers;
using project_backend.Interfaces;
using project_backend.Services;
using System.Reflection;
using System.Security.Cryptography;

namespace UnitTests.Services
{
    internal class AuthServiceTests
    {
        private readonly Mock<IUserAuthRepository> _userAuthRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly TokenInfo _tokenInfo;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userAuthRepositoryMock = new Mock<IUserAuthRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _configurationMock = new Mock<IConfiguration>();

            _tokenInfo = new TokenInfo(
                _configurationMock.Object,
                _httpContextAccessorMock.Object);

            _authService = new AuthService(
                _userAuthRepositoryMock.Object,
                _userRepositoryMock.Object,
                _httpContextAccessorMock.Object,
                _tokenInfo);
        }
        private static void SetPrivateFieldValue<T>(T obj, string fieldName, object value)
        {
            var field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(obj, value);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldGenerateTokenWithExpectedLength()
        {
            var refreshToken = _authService.GenerateRefreshToken();

            // Assert
            Assert.NotNull(refreshToken.Token);
            Assert.Equal(88, refreshToken.Token.Length); // Base64 encoding adds padding
        }

        [Fact]
        public void GenerateRefreshToken_ShouldGenerateValidExpirationTime()
        {
            var refreshToken = _authService.GenerateRefreshToken();

            // Assert
            Assert.True(refreshToken.Expires > refreshToken.Created);
            Assert.Equal(DateTime.UtcNow.AddMinutes(_tokenInfo.RefreshTokenExpires), refreshToken.Expires, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void GenerateRefreshToken_ShouldGenerateDifferentTokens()
        {
            var refreshToken1 = _authService.GenerateRefreshToken();
            var refreshToken2 = _authService.GenerateRefreshToken();

            // Assert
            Assert.NotEqual(refreshToken1.Token, refreshToken2.Token);
        }
    }
}
