using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TaskTracker.Services.Shared.Kafka;

public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
{
    private readonly ILogger<KafkaProducer<TMessage>> _logger;
    private readonly IProducer<string, TMessage> _producer;
    private readonly string _topic;
    
    public KafkaProducer(
        IOptions<KafkaOptions> kafkaOptions, 
        ILogger<KafkaProducer<TMessage>> logger)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaOptions.Value.BootstrapServers,
            EnableIdempotence = true,
            Acks = Acks.All,
            ClientId = "default-producer",
            MessageSendMaxRetries = 1,
            RetryBackoffMs = 100,
            MessageTimeoutMs = 2000
        };
        
        _logger = logger;
        
        _producer = new ProducerBuilder<string, TMessage>(config)
            .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
            .Build();

        _topic = kafkaOptions.Value.Topic;
    }
    
    public async Task ProduceAsync(string key, TMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await _producer.ProduceAsync(topic: _topic, new Message<string, TMessage>
            {
                Key = key,
                Value = message
            }, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке сообщения в топик {Topic}. Ключ: {Key}", _topic, key);
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }
}