using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_backend.DTOs.Requests;
using project_backend.Exceptions;
using project_backend.Interfaces;
using project_backend.Model.Entities;

namespace project_backend.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(NewUserRequest request)
    {
        try
        {
            int userId = await _authService.RegisterAsync(request);
        }
        catch (AuthenticationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
            //return Problem(detail: "Oops! There was an unexpected error during the user registration. Please try again. ");
        }

        return Created();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Authenticate(UserAuthRequest request)
    {
        try
        {
            UserAuth user = await _authService.AuthenticateAsync(request);

            string token = _authService.GenerateToken(user);

            return Ok(token);
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (AuthNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (AuthInvalidCredentialsException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
            //return Problem(detail: "Unexpected error occured during Authentication. Please try again.");
        }
    }
}
