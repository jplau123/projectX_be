using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IItemService
    {
        public List<Item> GetItems();
        //public int GetItemById(int id);
        //public int UpdateItem(int id, string name, decimal price, int amount);
        //public int DeleteItem(int id);
        //public int AddNewItem(int id, string name, decimal price, int amount);
        
    }
}
