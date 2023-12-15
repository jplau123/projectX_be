using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs.Requests;
using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IUserService userRepository,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(NewUserRequest request)
    {
        User user = await _authService.RegisterAsync(request);

        _logger.Log(LogLevel.Information, "User '{username}' successfuly registered.", request.UserName);

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
            if (HttpContext.User.Identity.IsAuthenticated)
                return BadRequest($"You are already authenticated as '{HttpContext.User.Identity.Name}'.");

        UserAuth user = await _authService.AuthenticateAsync(request);

        await _authService.SetAccessToken(user);

        await _authService.SetRefreshToken(user);

        _authService.SetTokenCookie();

        _authService.SetRefreshTokenCookie();

        _logger.Log(LogLevel.Information, "User '{username}' succesfully authenticated. ", user.User_Name);

        return Ok($"Succesfully authenticated as '{user.User_Name}'.");
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult LogOut()
    {
        _logger.Log(LogLevel.Information, "Attempting to log out. ");

        _authService.DeleteTokenCookie();

        _authService.DeleteRefreshTokenCookie();

        _logger.Log(LogLevel.Information, "Succesfully logged out. ");

        return Ok($"Succesfully loged out.");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Refresh()
    {
        _logger.Log(LogLevel.Information, "Atempting to refresh the token... ");

        await _authService.RefreshJwtToken();

        _logger.Log(LogLevel.Information, "Token succesfuly refreshed. ");

        return Ok();
    }
}
