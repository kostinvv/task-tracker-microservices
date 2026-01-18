using Microsoft.Extensions.DependencyInjection;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using TaskTracker.Services.Shared.Emails;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Services;

namespace TaskTracker.Services.Tasks.ApplicationCore;

public static class DependencyInjection
{
    public static void AddApplicationCore(
        this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskOrderService, TaskOrderService>();
        services.AddScoped<ITaskService, TaskService>();
        
        services.AddSingleton<IRazorEngineService>(_ =>
        {
            var templateServiceConfiguration = new TemplateServiceConfiguration();
            return RazorEngineService.Create(templateServiceConfiguration);
        });

        services.AddSingleton<IEmailTemplateService, RazorEmailTemplateService>();
    }
}