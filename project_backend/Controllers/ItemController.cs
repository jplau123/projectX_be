using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs.RequestDTO;
using project_backend.Exceptions;
using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            List<Item> itemsList = _itemService.GetItems();

            if (itemsList == null)
            {
                throw new NotFoundException("No items found.");
            }
            return Ok(itemsList);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            var result = await _itemService.GetItemById(id);
            if (result == null)
            {
                throw new NotFoundException("No items found.");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewItem(AddNewItem newItem)
        {
            var result = await _itemService.AddNewItem(newItem.Name, newItem.Price, newItem.Quantity, newItem.Created_By);
            if (result == null)
            {
                throw new FailedToAddException("Failed to add item");
            }
            return CreatedAtAction(nameof(GetItemById), new { id = result.Item_Id }, result);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItem updateItem)
        {
            var result = await _itemService.UpdateItem(id, updateItem.Name, updateItem.Price, updateItem.Quantity);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var result = await _itemService.DeleteItem(id);
            if (result == null)
            {
                throw new NotFoundException($"There is no item with id {id}");
            }
            return Ok(result);
        }
    }
}
