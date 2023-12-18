using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IItemRepository
    {
        public IEnumerable<Item> GetItems();
        public int GetItemQuantityInStore(string itemName);
        public decimal GetTotalItemPrice(string itemName, int quantityToBuy);
        public void DeleteItem(string itemName);
        public void UpdateItemQuantity(string itemName, int reducedQuantity);
        public Task<Item> GetItemById(int id);  
        public Task<bool> UpdateItem(int id, string name, decimal price, int quantity);
        public Task<bool> DeleteItem(int id);
        public Task<int> AddNewItem(string name, decimal price, int amount, string? created_by);

    }
}
