using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace KafkaWebApi.Services
{
    public class KafkaProducer
    {
        private readonly IProducer<Null, string> _producer;
        private const string BootstrapServers = "localhost:9092";
        private const string Topic = "cppweb";

        public KafkaProducer()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = BootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                var result = await _producer.ProduceAsync(Topic, new Message<Null, string> { Value = message });
                Console.WriteLine($"Delivered to {result.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}
