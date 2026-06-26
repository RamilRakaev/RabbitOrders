using Orders.Application.Constants;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Orders.Infrastructure.RabbitMq.Helpers
{
    public class MessageProducer(IChannel channel)
    {
        private readonly IChannel _channel = channel;

        public async Task Publish<Message>(Message message, string queueName) where Message : class
        {
            string orderJson = JsonSerializer.Serialize(message);
            byte[] orderBytes = Encoding.UTF8.GetBytes(orderJson);
            BasicProperties properties = new()
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
            };
            await _channel.BasicPublishAsync(RabbitMqNames.ExchangeName, queueName, true, properties, orderBytes);
        }
    }
}
