using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TaskTracker.Services.Tasks.Api.Controllers;

public class BaseController : ControllerBase
{
    protected string UserEmail => HttpContext.User.FindFirstValue(ClaimTypes.Email)!;
    protected Guid UserId => Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}