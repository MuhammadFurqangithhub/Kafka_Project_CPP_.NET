using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaWebApi
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly MessageStore _messageStore;
        private readonly string _topic = "cppweb";

        public KafkaConsumer(ILogger<KafkaConsumer> logger, MessageStore messageStore)
        {
            _logger = logger;
            _messageStore = messageStore;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = "cpp-consumer-group2",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe(_topic);

                _logger.LogInformation($"🟢 Kafka Consumer started on topic '{_topic}'.");

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var result = consumer.Consume(stoppingToken);
                            var message = result.Message.Value;

                            _logger.LogInformation($"Received: {message}");
                            _messageStore.AddMessage(message);
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError($"❌ Error consuming message: {ex.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("⚠️ Consumer cancellation requested.");
                }
                finally
                {
                    consumer.Close();
                    _logger.LogInformation("🔴 Kafka Consumer closed.");
                }
            });
        }
    }
}
