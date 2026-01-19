namespace TaskTracker.Services.Shared.Kafka;

public interface IKafkaProducer<in TMessage> : IDisposable
{
    Task ProduceAsync(string key, TMessage message, CancellationToken cancellationToken);
}