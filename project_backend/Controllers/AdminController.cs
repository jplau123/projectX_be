using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs.RequestDTO;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByUserIdAsync(int id)
        {
            var user = await _userService.GetUserByUserIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteUserByUserIdAsync(int id)
        {
            await _userService.DeleteUserByUserIdAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync([FromForm] AddUserRequest request)
        {
            await _userService.AddUserAsync(request);
            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserByUserIdAsync(UpdateUserRequest request)
        {
            return Ok(await _userService.UpdateUserByUserIdAsync(request));
        }
    }
}