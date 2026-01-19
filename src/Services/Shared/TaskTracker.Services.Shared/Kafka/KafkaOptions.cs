namespace TaskTracker.Services.Shared.Kafka;

public class KafkaOptions
{
    public const string SectionName = "Kafka";
    public const string TasksSectionName = "Tasks";
    
    public string BootstrapServers { get; init; } = null!;
    public string Topic { get; init; } = null!;
    
    public string GroupId { get; init; } = string.Empty;
}