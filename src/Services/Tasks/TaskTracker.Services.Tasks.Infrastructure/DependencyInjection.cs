using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Services.Shared.Kafka;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Auth;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Context;
using TaskTracker.Services.Tasks.Infrastructure.Auth;

namespace TaskTracker.Services.Tasks.Infrastructure;

public static class DependencyInjection
{
    public static void AddProducer<TMessage>(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.Configure<KafkaOptions>(configurationSection);
        services.AddSingleton<IKafkaProducer<TMessage>, KafkaProducer<TMessage>>();
    }

    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IJwtProvider, JwtProvider>();
    }
}