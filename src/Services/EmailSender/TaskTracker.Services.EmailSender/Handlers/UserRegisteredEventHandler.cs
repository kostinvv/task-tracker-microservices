using Microsoft.Extensions.Logging;
using TaskTracker.Services.Shared.Events.Users;

namespace TaskTracker.Services.EmailSender.Infrastructure.Handlers;

public class UserRegisteredEventHandler(
    ILogger<UserRegisteredEventHandler> logger) : IEventHandler<UserRegisteredEvent>
{
    public Task HandleAsync(string key, UserRegisteredEvent value, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Обработка сообщения UserRegisteredEvent. UserId={UserId}, MessageId={Key}", 
            value.UserId, 
            key);
        
        // TODO: Отправка приветственного сообщения пользователю на его E-mail.
        
        logger.LogInformation(
            "Сообщение UserRegisteredEvent обработано успешно. UserId={UserId}, MessageId={Key}", 
            value.UserId, 
            key);

        return Task.CompletedTask;
    }
}