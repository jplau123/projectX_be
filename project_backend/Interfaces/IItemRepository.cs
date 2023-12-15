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
    }
}
