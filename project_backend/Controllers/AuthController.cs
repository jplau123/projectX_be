using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs;

namespace project_backend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(NewUserRequest request)
    {
        if(request.UserName.Length == 0)
        {
            return BadRequest();
        }

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(UserAuthRequest request)
    {
        return Ok();
    }
}
