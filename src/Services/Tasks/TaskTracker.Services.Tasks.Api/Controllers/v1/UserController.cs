using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Services.Shared.Results;
using TaskTracker.Services.Tasks.Api.Contracts.v1.Users;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs;

namespace TaskTracker.Services.Tasks.Api.Controllers.v1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/user")]
public class UserController(IUserService userService) : BaseController
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
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken = default)
    {
        var result = await userService.RegisterAsync(email: registerRequest.Email, password: registerRequest.Password, cancellationToken);
        
        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }
    
    /// <summary>
    /// Авторизация пользователя.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        var result = await userService.LoginAsync(email: loginRequest.Email, password: loginRequest.Password, cancellationToken);
        
        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }
}
