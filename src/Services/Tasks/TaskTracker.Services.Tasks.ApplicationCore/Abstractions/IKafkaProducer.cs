namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions;

public interface IKafkaProducer<in TMessage> : IDisposable
{
    Task ProduceAsync(TMessage message, CancellationToken cancellationToken);
}