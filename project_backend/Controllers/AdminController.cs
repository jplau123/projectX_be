using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs.RequestDTO;
using project_backend.Enums;
using project_backend.Interfaces;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var usersList = await _userService.GetUsersAsync();
            return Ok(usersList);
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByUserIdAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteUserByUserIdAsync(int id)
        {
            await _userService.DeleteUserByIdAsync(id);
            return Ok();
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost]
        public async Task<IActionResult> AddUserAsync([FromBody] AddUserRequest request)
        {
            await _userService.AddUserAsync(request);
            return Created();
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPut]
        public async Task<IActionResult> UpdateUserByUserIdAsync([FromBody] UpdateUserRequest request)
        {
            return Ok(await _userService.UpdateUserByIdAsync(request));
        }
    }
}