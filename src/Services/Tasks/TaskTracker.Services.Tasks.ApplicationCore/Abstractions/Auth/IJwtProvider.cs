using TaskTracker.Services.Shared.Data.Entities;
using TaskTracker.Services.Shared.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Auth;

public interface IJwtProvider
{
    public string GenerateJwtToken(ApplicationUser user);
}