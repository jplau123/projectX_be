using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs.RequestDTO;
using project_backend.DTOs.ResponseDTO;
using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ILogger<AuthController> logger,
        ILoginService loginService)
    {
        _logger = logger;
        _loginService = loginService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(NewUserRequest request)
    {
        User user = await _loginService.Register(request);

        _logger.Log(LogLevel.Information, "User '{username}' successfuly registered.", request.UserName);

        return Created("https://localhost:7025/User/GetUserById", user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(UserAuthRequest request)
    {
        _logger.Log(LogLevel.Information, "Attempting to authenticate... ");

        TokenResponse token = await _loginService.Authenticate(request);

        _logger.Log(LogLevel.Information, "User '{username}' succesfully authenticated. ", request.UserName);

        return Ok(token);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        _logger.Log(LogLevel.Information, "Attempting to log out. ");

        await _loginService.RevokeAccess();

        _logger.Log(LogLevel.Information, "Succesfully logged out. ");

        return Ok($"Succesfully loged out.");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody] TokenRequest? request)
    {
        _logger.Log(LogLevel.Information, "Atempting to refresh the token... ");

        TokenResponse newToken = await _loginService.RefreshAccess(request);

        _logger.Log(LogLevel.Information, "Token succesfuly refreshed. ");

        return Ok(newToken);
    }
}
