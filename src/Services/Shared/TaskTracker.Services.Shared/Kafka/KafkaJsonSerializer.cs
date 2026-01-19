using System.Text.Json;
using Confluent.Kafka;

namespace TaskTracker.Services.Shared.Kafka;

public class KafkaJsonSerializer<TMessage> : ISerializer<TMessage>
{
    public byte[] Serialize(TMessage data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}