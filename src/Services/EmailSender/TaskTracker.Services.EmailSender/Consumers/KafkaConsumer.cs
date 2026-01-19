using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskTracker.Services.EmailSender.Abstractions;
using TaskTracker.Services.Shared.Kafka;

namespace TaskTracker.Services.EmailSender.Consumers;

public class KafkaConsumer<TMessage> : BackgroundService
{
    private readonly ILogger<KafkaConsumer<TMessage>> _logger;
    private readonly IConsumer<string, TMessage> _consumer;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _topic;

    public KafkaConsumer(
        ILogger<KafkaConsumer<TMessage>> logger,
        IOptions<KafkaOptions> options,
        IServiceProvider serviceProvider)
    {
        var option = options.Value;
        _topic = option.Topic;
        _logger = logger;
        _serviceProvider = serviceProvider;
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = option.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            GroupId = option.GroupId,
            EnableAutoCommit = false,
            FetchWaitMaxMs = 5,
            FetchMaxBytes = 52428800
        };  
        
        _consumer = new ConsumerBuilder<string, TMessage>(consumerConfig)
            .SetValueDeserializer(new KafkaValueDeserializer<TMessage>())
            .Build();
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        try
        {
            _consumer.Subscribe(_topic);
            _logger.LogInformation("Подписка на топик {Topic} выполнена", _topic);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, TMessage> result;
                
                try
                {
                    result = _consumer.Consume(stoppingToken);
                }                
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Ошибка Consume: {Reason}", ex.Error.Reason);
                    if (ex.Error.IsFatal) throw;
                    continue;
                }

                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TMessage>>();
                
                try
                {
                    await handler.HandleAsync(
                        result.Message.Key,
                        result.Message.Value,
                        stoppingToken);

                    // Производим коммит только после успешной обработки сообщения.
                    _consumer.Commit(result);
                }
                catch (KafkaException ex)
                {
                    _logger.LogWarning(ex, "Ошибка Kafka при commit: {Reason}", ex.Error.Reason);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка Handler: {Message}", ex.Message);
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _consumer.Close();
        }
    }
    
    public override void Dispose()
    {
        _consumer.Dispose();
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}