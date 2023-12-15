using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs.Requests;
using project_backend.Helpers;
using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userRepository;

    public AuthController(IAuthService authService, IUserService userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(NewUserRequest request)
    {
        User user = await _authService.RegisterAsync(request);

        return CreatedAtAction(
            controllerName: "User",
            actionName: nameof(UserController.GetUserById),
            routeValues: new { user_id = user.User_Id },
            value: user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Authenticate(UserAuthRequest request)
    {
        if (HttpContext.User.Identity != null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return Ok($"You are already authenticated as '{HttpContext.User.Identity.Name}'.");
        }

        UserAuth user = await _authService.AuthenticateAsync(request);

        string token = _authService.GenerateToken(user);

        CookieHelper.SetTokenCookie(HttpContext, token);

        return Ok($"Succesfully authenticated as '{user.User_Name}'.");
    }
}
