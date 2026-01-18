using Microsoft.Extensions.Logging;
using TaskTracker.Services.EmailSender.Abstractions;
using TaskTracker.Services.EmailSender.DTOs;
using TaskTracker.Services.Shared.Emails;

namespace TaskTracker.Services.EmailSender.Handlers;

public class EmailNotificationEventHandler(
    ILogger<EmailNotificationEventHandler> logger,
    IEmailService emailService) : IEventHandler<EmailNotificationEvent>
{
    public async Task HandleAsync(string key, EmailNotificationEvent emailEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Обработка события EmailNotificationEvent. MessageId={Key}", 
            key);
        
        await emailService.SendEmailAsync(
            emailDto: new EmailDto(emailEvent.Email, emailEvent.Subject, emailEvent.Body), 
            cancellationToken);
        
        logger.LogInformation(
            "Событие EmailNotificationEvent обработано успешно. MessageId={Key}", 
            key);
    }
}