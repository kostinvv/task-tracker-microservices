using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Auth;

public interface IJwtProvider
{
    public string GenerateJwtToken(ApplicationUser user);
}