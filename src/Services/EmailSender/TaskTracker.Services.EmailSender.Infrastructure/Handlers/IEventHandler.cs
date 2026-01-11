namespace TaskTracker.Services.EmailSender.Infrastructure.Handlers;

public interface IEventHandler<in TMessage>
{
    Task HandleAsync(string key, TMessage value, CancellationToken cancellationToken);
}