using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Services.Tasks.Api.Contracts.v1.Users;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Auth;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.Api.Controllers.v1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/user")]
public class UserController(
    IJwtProvider jwtProvider, 
    UserManager<ApplicationUser> userManager) : BaseController
{
    /// <summary>
    /// Получение текущего пользователя.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new UserResponse(Id: UserId, Email: UserEmail));

    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
    {
        ApplicationUser applicationUser = new()
        { 
            UserName = registerRequest.Email,
            Email = registerRequest.Email
        };
        
        var identityResult = await userManager.CreateAsync(user: applicationUser, password: registerRequest.Password);

        if (!identityResult.Succeeded)
        {
            return Problem(
                detail: identityResult.Errors.First().Description,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
        
        var jwtToken = jwtProvider.GenerateJwtToken(applicationUser);
        var response = new AuthenticationResult(jwtToken);
        return Ok(response);
    }
    
    /// <summary>
    /// Авторизация пользователя.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
    {
        var applicationUser = await userManager.FindByEmailAsync(loginRequest.Email);
        
        if (applicationUser is null)
        {
            return NotFound();
        }
        
        var isPasswordValid = await userManager.CheckPasswordAsync(user: applicationUser, password: loginRequest.Password);
        
        if (!isPasswordValid)
        {
            return Problem(
                detail: "Invalid user or password.",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }
        
        var jwtToken = jwtProvider.GenerateJwtToken(applicationUser);
        var response = new AuthenticationResult(jwtToken);
        return Ok(response);
    }
}
