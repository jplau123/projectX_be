using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IItemService
    {
        public List<Item> GetItems();
        public Task<Item> GetItemById(int id);
        public Task<Item> UpdateItem(int id, string name, decimal price, int quantity);
        public Task<bool> DeleteItem(int id);
        public Task<Item> AddNewItem(string name, decimal price, int amount, string? created_By);
    }
}
