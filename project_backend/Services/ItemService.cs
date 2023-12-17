using Microsoft.AspNetCore.Http.HttpResults;
using project_backend.Exceptions;
using project_backend.Interfaces;
using project_backend.Model.Entities;
using System.Xml.Linq;

namespace project_backend.Services
{
    public class ItemService : IItemService
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

        public Task<Item> GetItemById(int id)
        {
            return _itemRepository.GetItemById(id);
        }

        public async Task<Item> AddNewItem(string name, decimal price, int amount, string? created_by)
        {
            bool itemExists = await _itemRepository.CheckIfItemExists(name);
            if (itemExists)
            {
                throw new ItemAlreadyExistsException("Item already exists");
            }

            var id = await _itemRepository.AddNewItem(name, price, amount, created_by);
            return await _itemRepository.GetItemById(id);
        }

        public async Task<Item> UpdateItem(int id, string name, decimal price, int quantity)
        {
            var result = await _itemRepository.UpdateItem(id, name, price, quantity);
            if (result)
            {
                return await _itemRepository.GetItemById(id);
            }
            throw new InvalidOperationException();
        }

        public async Task<bool> DeleteItem(int id)
        {
            bool itemExists = await _itemRepository.CheckIfItemExistsById(id);
            if (itemExists)
            {
                throw new ItemAlreadyExistsException("Item already exists");// problema!! 
            }
            return await _itemRepository.DeleteItem(id);
        }

        public Task<bool> CheckIfItemExists(string name)
        {
           return _itemRepository.CheckIfItemExists(name);
        }

        public Task<bool> CheckIfItemExistsById(int id)
        {
            return _itemRepository.CheckIfItemExistsById(id);
        }
        
    }
}
