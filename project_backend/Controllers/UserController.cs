using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddUserBalance(int user_id, int balance)
        {
            
            return Ok("The new balance is $" + _userService.AddUserBalance(user_id, balance));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUserById(int user_id)
        {
            return Ok(await _userService.GetUserById(user_id));
        }

    }
}
