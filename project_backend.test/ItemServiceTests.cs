using Moq;
using project_backend.Services;
using project_backend.Model.Entities;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using project_backend.Interfaces;

public class ItemServiceTests
{
    private readonly Mock<IItemRepository> _mockItemRepo;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _mockItemRepo = new Mock<IItemRepository>();
        _itemService = new ItemService(_mockItemRepo.Object);
    }

    [Fact]
    public void GetItems_ShouldReturnItems_WhenItemsExist()
    {
        // Arrange
        var items = new List<Item> { new Item(), new Item() };
        _mockItemRepo.Setup(repo => repo.GetItems()).Returns(items.AsQueryable());

        // Act
        var result = _itemService.GetItems();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetItems_ShouldReturnNull_WhenNoItemsExist()
    {
        // Arrange
        var items = new List<Item>();
        _mockItemRepo.Setup(repo => repo.GetItems()).Returns(items.AsQueryable());

        // Act
        var result = _itemService.GetItems();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetItemById_ShouldReturnItem_WhenItemExists()
    {
        // Arrange
        var itemId = 1;
        var item = new Item { Item_Id = itemId };
        _mockItemRepo.Setup(repo => repo.GetItemById(itemId)).ReturnsAsync(item);

        // Act
        var result = await _itemService.GetItemById(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(itemId, result.Item_Id);
    }

    [Fact]
    public async Task AddNewItem_ShouldReturnNewItem()
    {
        // Arrange
        var newItem = new Item { Item_Name = "NewItem", Price = 100, Quantity = 10 };
        _mockItemRepo.Setup(repo => repo.AddNewItem(newItem.Item_Name, newItem.Price, newItem.Quantity, null)).ReturnsAsync(1);
        _mockItemRepo.Setup(repo => repo.GetItemById(1)).ReturnsAsync(newItem);

        // Act
        var result = await _itemService.AddNewItem(newItem.Item_Name, newItem.Price, newItem.Quantity, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newItem.Item_Name, result.Item_Name);
    }

    [Fact]
    public async Task UpdateItem_ShouldReturnUpdatedItem()
    {
        // Arrange
        var updatedItem = new Item { Item_Id = 1, Item_Name = "UpdatedItem", Price = 200, Quantity = 20 };
        _mockItemRepo.Setup(repo => repo.UpdateItem(updatedItem.Item_Id, updatedItem.Item_Name, updatedItem.Price, updatedItem.Quantity)).ReturnsAsync(true);
        _mockItemRepo.Setup(repo => repo.GetItemById(updatedItem.Item_Id)).ReturnsAsync(updatedItem);

        // Act
        var result = await _itemService.UpdateItem(updatedItem.Item_Id, updatedItem.Item_Name, updatedItem.Price, updatedItem.Quantity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedItem.Item_Name, result.Item_Name);
    }

    [Fact]
    public async Task UpdateItem_ShouldThrowInvalidOperationException_WhenUpdateFails()
    {
        // Arrange
        var itemId = 1;
        _mockItemRepo.Setup(repo => repo.UpdateItem(itemId, It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _itemService.UpdateItem(itemId, "NewItem", 100, 10));
    }

    [Fact]
    public async Task DeleteItem_ShouldReturnTrue_WhenDeletionIsSuccessful()
    {
        // Arrange
        var itemId = 1;
        _mockItemRepo.Setup(repo => repo.DeleteItem(itemId)).ReturnsAsync(true);

        // Act
        var result = await _itemService.DeleteItem(itemId);

        // Assert
        Assert.True(result);
    }
    [Fact]
    public async Task GetItemById_ShouldReturnNull_WhenItemDoesNotExist()
    {
        // Arrange
        var itemId = 99; // Assuming this ID does not exist
        _mockItemRepo.Setup(repo => repo.GetItemById(itemId)).ReturnsAsync((Item)null);

        // Act
        var result = await _itemService.GetItemById(itemId);

        // Assert
        Assert.Null(result);
    }
    [Fact]
    public async Task AddNewItem_ShouldThrowException_WhenItemAlreadyExists()
    {
        // Arrange
        var newItem = new Item { Item_Name = "ExistingItem", Price = 100, Quantity = 10 };
        _mockItemRepo.Setup(repo => repo.AddNewItem(newItem.Item_Name, newItem.Price, newItem.Quantity, null))
                     .ThrowsAsync(new InvalidOperationException("Item already exists."));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _itemService.AddNewItem(newItem.Item_Name, newItem.Price, newItem.Quantity, null));
    }
    [Fact]
    public async Task DeleteItem_ShouldReturnFalse_WhenItemDoesNotExist()
    {
        // Arrange
        var itemId = 99; // Assuming this ID does not exist
        _mockItemRepo.Setup(repo => repo.DeleteItem(itemId)).ReturnsAsync(false);

        // Act
        var result = await _itemService.DeleteItem(itemId);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public async Task UpdateItem_ShouldThrowInvalidOperationException_WhenItemDoesNotExist()
    {
        // Arrange
        var itemId = 99; // Non-existent item ID
        _mockItemRepo.Setup(repo => repo.UpdateItem(itemId, It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _itemService.UpdateItem(itemId, "NewItem", 100, 10));
    }
}
