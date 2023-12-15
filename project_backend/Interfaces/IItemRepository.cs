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
        //public int GetItemById(int id);
        public Task<int> UpdateItem(int id, string name, decimal price, int quantity);
        public int DeleteItem(int id);
        public Task<int> AddNewItem(int id, string name, decimal price, int quantity, string? created_by);

    }
}
