using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs.RequestDTO;
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
                return NotFound();
            }

            return Ok(itemsList);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewItem([FromBody] AddNewItem newItem)
        {
            var result = _itemService.AddNewItem(newItem.Id, newItem.Name, newItem.Price, newItem.Quantity, newItem.Created_By);
            return Ok(await Task.FromResult(result));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateItem updateItem)
        {
            var result = _itemService.UpdateItem(updateItem.Id, updateItem.Name, updateItem.Price, updateItem.Quantity);
            return Ok(await Task.FromResult(result));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteItem(int id)
        {
            return Ok(_itemService.DeleteItem(id));
        }
    }
}
