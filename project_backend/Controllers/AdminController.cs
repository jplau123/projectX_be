using Microsoft.AspNetCore.Mvc;
using project_backend.Exceptions;
using project_backend.Interfaces;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var usersList = await _userService.GetUsersAsync();

            if (usersList == null)
            {
                throw new NotFoundException("No users found.");
            }

            return Ok(usersList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteUserByUserIdAsync(int id)
        {
            await _userService.DeleteUserByUserIdAsync(id);
            return Ok();
        }
    }
}
