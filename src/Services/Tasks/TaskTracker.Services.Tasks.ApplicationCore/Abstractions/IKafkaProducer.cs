namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions;

public interface IKafkaProducer<in TMessage> : IDisposable
{
    Task ProduceAsync(string key, TMessage message, CancellationToken cancellationToken);
}