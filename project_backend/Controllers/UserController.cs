using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_backend.Enums;
using project_backend.Interfaces;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut]
        public async Task<IActionResult> PurchaseItem(int userId, string itemName, int quantityToBuy)
        {
            _userService.PurchaseItem(userId, itemName, quantityToBuy);
            return Ok($"{itemName} purchase successful");
        }

        [HttpPut]
        public async Task<IActionResult> AddUserBalance(int user_id, decimal balance)
        {
            return Ok("The new balance is $" + _userService.AddUserBalance(user_id, balance));
        }

        [Authorize(Roles = nameof(Roles.Manager))]
        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseHistoryAsync()
        {
            var purchaseHistory = await _userService.GetAllPurchaseHistoryAsync();
            return Ok(purchaseHistory);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByIdAsync(int user_id)
        {
            return Ok(await _userService.GetUserByIdAsync(user_id));
        }

    }
}
