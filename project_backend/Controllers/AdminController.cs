using Microsoft.AspNetCore.Mvc;
using project_backend.Exceptions;
using project_backend.Interfaces;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminController(IAdminService adminService) : ControllerBase
    {
        private readonly IAdminService _adminService = adminService;

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var usersList = await _adminService.GetUsersAsync();

            if (usersList == null)
            {
                throw new NotFoundException("No users found.");
            }

            return Ok(usersList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteUserByUserIdAsync(int id)
        {
            var deletedUsersCount = await _adminService.DeleteUserByUserIdAsync(id);

            if (deletedUsersCount == -1 || deletedUsersCount == -2)
            {
                throw new NotFoundException("Could not find user with provided id.");
            }

            if (deletedUsersCount == 0)
            {
                throw new Exception("Internal error, user was found, but not deleted.");
            }

            return Ok();
        }
    }
}
