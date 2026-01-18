using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Services.EmailSender.Abstractions;
using TaskTracker.Services.EmailSender.Consumers;
using TaskTracker.Services.EmailSender.Handlers;
using TaskTracker.Services.EmailSender.Options;
using TaskTracker.Services.EmailSender.Services;
using TaskTracker.Services.Shared.Emails;

namespace TaskTracker.Services.EmailSender;

public static class DependencyInjection
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(
            configuration.GetSection(key: SmtpOptions.SectionName));
        
        services.AddScoped<IEmailService, EmailService>();
        
        services.AddConsumer<EmailNotificationEvent, EmailNotificationEventHandler>(
            section: configuration.GetSection(KafkaOptions.SectionName));
    }
    
    private static void AddConsumer<TMessage, THandler>(
        this IServiceCollection services,
        IConfigurationSection section) where THandler : class, IEventHandler<TMessage>
    {
        services.Configure<KafkaOptions>(section);
        services.AddHostedService<KafkaConsumer<TMessage>>();
        services.AddScoped<IEventHandler<TMessage>, THandler>();
    }
}