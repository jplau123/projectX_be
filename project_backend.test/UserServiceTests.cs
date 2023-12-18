using Moq;
using project_backend.Services;
using project_backend.Model.Entities;
using project_backend.Interfaces;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using project_backend.Exceptions;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IItemRepository> _mockItemRepo;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockItemRepo = new Mock<IItemRepository>();
        _userService = new UserService(_mockUserRepo.Object, _mockItemRepo.Object);
    }

    [Fact]
    public void AddUserBalance_ShouldReturnUpdatedBalance()
    {
        // Arrange
        var userId = 1;
        var initialBalance = 100m;
        var topUpAmount = 50m;
        var expectedBalance = 150m;

        _mockUserRepo.Setup(repo => repo.GetUserBalance(userId)).Returns(initialBalance);
        _mockUserRepo.Setup(repo => repo.AddUserBalance(userId, It.IsAny<decimal>())).Returns(expectedBalance);

        // Act
        var result = _userService.AddUserBalance(userId, topUpAmount);

        // Assert
        Assert.Equal(expectedBalance, result);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new User { /* Initialize properties */ };

        _mockUserRepo.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        Assert.Equal(expectedUser, result);
    }

    [Fact]
    public async Task GetUserById_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 1;
        _mockUserRepo.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserById(userId));
    }
    [Fact]
    public void PurchaseItem_ShouldCompletePurchase_WhenBalanceAndQuantitySufficient()
    {
        // Arrange
        var userId = 1;
        var itemName = "Item1";
        var quantityToBuy = 5;
        var userBalance = 200m;
        var unitPrice = 20m;
        var totalPrice = unitPrice * quantityToBuy;
        var quantityInStore = 10;

        _mockUserRepo.Setup(repo => repo.GetUserBalance(userId)).Returns(userBalance);
        _mockItemRepo.Setup(repo => repo.GetItemQuantityInStore(itemName)).Returns(quantityInStore);
        _mockItemRepo.Setup(repo => repo.GetTotalItemPrice(itemName, quantityToBuy)).Returns(totalPrice);

        // Act
        _userService.PurchaseItem(userId, itemName, quantityToBuy);

        // Assert
        _mockUserRepo.Verify(repo => repo.UpdateUserBalance(userId, It.IsAny<decimal>()), Times.Once);
        _mockUserRepo.Verify(repo => repo.AppendPurchaseHistory(userId, itemName, quantityToBuy, unitPrice), Times.Once);
        _mockItemRepo.Verify(repo => repo.UpdateItemQuantity(itemName, It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public void PurchaseItem_ShouldThrowExceededAmountException_WhenQuantityInsufficient()
    {
        // Arrange
        var userId = 1;
        var itemName = "Item1";
        var quantityToBuy = 5;
        var quantityInStore = 3;

        _mockItemRepo.Setup(repo => repo.GetItemQuantityInStore(itemName)).Returns(quantityInStore);

        // Act & Assert
        Assert.Throws<ExceededAmountException>(() => _userService.PurchaseItem(userId, itemName, quantityToBuy));
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnListOfUsers()
    {
        // Arrange
        var users = new List<User> { new User(), new User() }; // Add mock users
        _mockUserRepo.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersAsync();

        // Assert
        Assert.Equal(users.Count, result.Count);
    }
    [Fact]
    public async Task DeleteUserByUserIdAsync_ShouldDeleteUser_WhenUserExistsAndNotDeleted()
    {
        // Arrange
        var userId = 1;
        var user = new User { Is_Deleted = false };

        _mockUserRepo.Setup(repo => repo.GetUserByUserIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _userService.DeleteUserByUserIdAsync(userId);

        // Assert
        _mockUserRepo.Verify(repo => repo.DeleteUserByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUserByUserIdAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 1;
        _mockUserRepo.Setup(repo => repo.GetUserByUserIdAsync(userId)).ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteUserByUserIdAsync(userId));
    }

    [Fact]
    public async Task DeleteUserByUserIdAsync_ShouldThrowAlreadySoftDeletedException_WhenUserAlreadyDeleted()
    {
        // Arrange
        var userId = 1;
        var user = new User { Is_Deleted = true };

        _mockUserRepo.Setup(repo => repo.GetUserByUserIdAsync(userId)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<AlreadySoftDeletedException>(() => _userService.DeleteUserByUserIdAsync(userId));
    }

}

