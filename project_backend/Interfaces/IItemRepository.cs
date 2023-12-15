using project_backend.Model.Entities;

namespace project_backend.Interfaces
{
    public interface IItemRepository
    {
        public IEnumerable<Item> GetItems();
        //public int GetItemById(int id);
        //public int UpdateItem(int id, string name, decimal price, int amount);
        //public int DeleteItem(int id);
        //public int AddNewItem(int id, string name, decimal price, int amount);

        public int GetItemAmountInStore(string item_name);
        public int GetTotalItemPrice(string item_name, int quantityToBuy);

        public void DeleteItem(string item_name);

        public void ReduceItemQuantity(string item_name, int reducedQuantity);
    }
}
