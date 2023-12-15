using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_backend.Enums;
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
        [Authorize(Roles = nameof(Roles.User))]
        public IActionResult GetItems()
        {
            List<Item> itemsList = _itemService.GetItems();

            if (itemsList == null)
            {
                return NotFound();
            }

            return Ok(itemsList);
        }
    }
}
