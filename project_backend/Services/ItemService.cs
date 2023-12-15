using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Services
{
    public class ItemService : IItemService //gali ir daugiau funkciju daryti. uz sesija, uz shopping carta (listas produktu, basket, patikrint ar yra prekes ir kiek ju) cart service (vieta, kuri turi patikrint, is shopping cart i history, sandely sumazint kieki ir pinigus nuskaiciuot is userio gal reik atskiro serviso)
    {
        private readonly IItemRepository _itemRepository;
        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public List<Item> GetItems()
        {
            var itemsList = _itemRepository.GetItems().ToList();
            if (itemsList.Count == 0)
            {
                return null;
            }
            return itemsList;
        }
        public async Task<bool> AddNewItem(int id, string name, decimal price, int quantity, string? created_by)
        {
            return await _itemRepository.AddNewItem(id, name, price, quantity, created_by) > 0;
        }
        public async Task<bool> UpdateItem(int id, string name, decimal price, int quantity)
        {
            return await _itemRepository.UpdateItem(id, name, price, quantity) > 0;
        }
        public bool DeleteItem(int id)
        {
            return _itemRepository.DeleteItem(id) > 0;
        }
    }
}
