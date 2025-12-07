using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Services.Shared.Results;

namespace TaskTracker.Services.Tasks.Api.Controllers;

public class BaseController : ControllerBase
{
    protected string UserEmail => HttpContext.User.FindFirstValue(ClaimTypes.Email)!;
    protected Guid UserId => Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected IActionResult Problem(Error error)
    {
        var statusCode = error.ErrorType switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.AccessUnAuthorized => StatusCodes.Status401Unauthorized,
            ErrorType.AccessForbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
        
        return Problem(statusCode: statusCode, title: error.Description, detail: error.Code);
    }
}