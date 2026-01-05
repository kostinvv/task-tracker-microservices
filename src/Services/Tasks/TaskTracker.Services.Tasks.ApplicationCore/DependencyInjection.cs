using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Services;

namespace TaskTracker.Services.Tasks.ApplicationCore;

public static class DependencyInjection
{
    public static void AddApplicationCore(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskService, TaskService>();
    }
}