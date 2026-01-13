namespace TaskTracker.Services.EmailSender.Infrastructure;

public class KafkaOptions
{
    public const string SectionName = "Kafka";
    
    public string BootstrapServers { get; init; } = null!;

    public string Topic { get; init; } = null!;

    public string GroupId { get; init; } = null!;
}