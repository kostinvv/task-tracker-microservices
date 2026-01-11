using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Services.EmailSender.Infrastructure.Consumers;
using TaskTracker.Services.EmailSender.Infrastructure.Handlers;

namespace TaskTracker.Services.EmailSender.Infrastructure;

public static class DependencyInjection
{
    public static void AddConsumer<TMessage, THandler>(
        this IServiceCollection services,
        IConfigurationSection section) where THandler : class, IEventHandler<TMessage>
    {
        services.Configure<KafkaOptions>(section);
        services.AddHostedService<KafkaConsumer<TMessage>>();
        services.AddSingleton<IEventHandler<TMessage>, THandler>();
    }
}