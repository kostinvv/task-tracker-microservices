using TaskTracker.Services.Shared.Results;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs;

namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions;

public interface IUserService
{
    Task<ResultT<AuthenticationResult>> RegisterAsync(string email, string password, CancellationToken cancellationToken);
    Task<ResultT<AuthenticationResult>> LoginAsync(string email, string password, CancellationToken cancellationToken);
}