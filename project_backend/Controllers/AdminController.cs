using Microsoft.AspNetCore.Mvc;
using project_backend.Interfaces;
using Serilog;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminController(IAdminService adminService) : Controller
    {
        private readonly IAdminService _adminService = adminService;

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var usersList = _adminService.GetUsers();

                if (usersList == null)
                {
                    return NotFound();
                }

                return Ok(usersList);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured: {ErrorMessage}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
