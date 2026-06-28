using Microsoft.Extensions.Options;
using Orders.Application.Models.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Orders.Infrastructure.RabbitMq.Helpers
{
    public class MessageProducer(IChannel channel, IOptions<RabbitMqTopologyOptions> options)
    {
        private readonly IChannel _channel = channel;
        private readonly RabbitMqTopologyOptions _options = options.Value;

        public async Task Publish<Message>(Message message, string queueName) where Message : class
        {
            string orderJson = JsonSerializer.Serialize(message);
            byte[] orderBytes = Encoding.UTF8.GetBytes(orderJson);
            BasicProperties properties = new()
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
            };
            await _channel.BasicPublishAsync(_options.ExchangeName, queueName, true, properties, orderBytes);
        }
    }
}
