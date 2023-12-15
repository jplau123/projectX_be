using Microsoft.AspNetCore.Mvc;
using project_backend.Interfaces;

namespace project_backend.Controllers
{


    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        
        
        public UserController(IUserService userService, IItemService itemService)
        {
            _userService = userService;
           

        }

        [HttpPut]

        public async Task<IActionResult> AddUserBalance(int user_id, int balance)
        {
            
            return Ok("The new balance is $" + _userService.AddUserBalance(user_id, balance));
        }

        [HttpPut]
        public async Task<IActionResult> PurchaseItem(int user_id, string item_name, int quantityToBuy)
        {
            _userService.PurchaseItem(user_id, item_name, quantityToBuy);
            return Ok($"{item_name} purchase successful");
        }
    }
}
