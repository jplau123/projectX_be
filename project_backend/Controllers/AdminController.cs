using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs.RequestDTO;
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
            return Ok(usersList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByUserIdAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteUserByUserIdAsync(int id)
        {
            await _userService.DeleteUserByIdAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync([FromBody] AddUserRequest request)
        {
            await _userService.AddUserAsync(request);
            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserByUserIdAsync([FromBody] UpdateUserRequest request)
        {
            return Ok(await _userService.UpdateUserByIdAsync(request));
        }
    }
}