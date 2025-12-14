namespace TaskTracker.Services.Tasks.Infrastructure.Kafka;

public class KafkaOptions
{
    public string BootstrapServers { get; init; } = null!;
    public string Topic { get; init; } = null!;
}