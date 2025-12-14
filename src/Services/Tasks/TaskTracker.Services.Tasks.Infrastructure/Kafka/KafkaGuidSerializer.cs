using System.Text.Json;
using Confluent.Kafka;

namespace TaskTracker.Services.Tasks.Infrastructure.Kafka;

public class KafkaGuidSerializer : ISerializer<Guid>
{
    public byte[] Serialize(Guid data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data.ToString());
    }
}