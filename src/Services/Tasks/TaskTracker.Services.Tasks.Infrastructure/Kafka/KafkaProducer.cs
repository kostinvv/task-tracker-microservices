using Confluent.Kafka;
using Microsoft.Extensions.Options;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;

namespace TaskTracker.Services.Tasks.Infrastructure.Kafka;

public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
{
    private readonly IProducer<Guid, TMessage> _producer;
    private readonly string _topic;
    
    public KafkaProducer(IOptions<KafkaOptions> kafkaOptions)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaOptions.Value.BootstrapServers
        };
        
        _producer = new ProducerBuilder<Guid, TMessage>(config)
            .SetKeySerializer(new KafkaGuidSerializer())
            .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
            .Build();

        _topic = kafkaOptions.Value.Topic;
    }
    
    public async Task ProduceAsync(TMessage message, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(topic: _topic, new Message<Guid, TMessage>()
        {
            Key = Guid.NewGuid(),
            Value = message
        }, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}