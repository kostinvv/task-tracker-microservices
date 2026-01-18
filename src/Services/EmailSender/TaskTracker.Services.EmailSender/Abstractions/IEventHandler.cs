namespace TaskTracker.Services.EmailSender.Abstractions;

public interface IEventHandler<in TMessage>
{
    Task HandleAsync(string key, TMessage message, CancellationToken cancellationToken);
}