using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IItemService
    {
        public List<Item> GetItems();
        //public int GetItemById(int id);
        public Task<bool> UpdateItem(int id, string name, decimal price, int quantity);
        public bool DeleteItem(int id);
        public Task<bool> AddNewItem(int id, string name, decimal price, int quantity, string? created_By);
    }
}
