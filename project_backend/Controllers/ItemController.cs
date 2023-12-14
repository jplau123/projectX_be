using Microsoft.AspNetCore.Mvc;
using project_backend.Interfaces;
using Serilog;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ItemController : BaseController
    {
        private readonly IItemService _itemService;
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public IActionResult GetItems()
        {
            try
            {
                var itemsList = _itemService.GetItems();

                if (itemsList == null)
                {
                    return NotFound();
                }

                return Ok(itemsList);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured: {ErrorMessage}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
