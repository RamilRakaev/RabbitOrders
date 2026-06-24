using Orders.Application.Constants;
using Orders.Application.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Orders.Api.Services
{
    public class OrderCreationProducer : IAsyncDisposable
    {
        private IConnection _connection;
        private IChannel _channel;

        public async Task PublishSuccessfulCreation(Order createdOrder)
        {
            await LasyInitialization();
            Message message = new()
            {
                EventType = EventTypes.OrderCreated,
                OrderId = createdOrder.Id,
                CreatedAt = DateTime.UtcNow,
            };
            await Publish(message);
        }

        public async Task PublishFailedCreation(Order createdOrder)
        {
            await LasyInitialization();
            Message message = new()
            {
                EventType = EventTypes.OrderFailed,
                CreatedAt = DateTime.UtcNow,
            };
            await Publish(message);
        }

        private async Task Publish(Message message)
        {
            string orderJson = JsonSerializer.Serialize(message);
            byte[] orderBytes = Encoding.UTF8.GetBytes(orderJson);
            BasicProperties properties = new()
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
            };

            await _channel.BasicPublishAsync(RabbitMqNames.ExchangeName, RabbitMqNames.OrderCreatedQueueName, true, properties, orderBytes);
        }

        private async Task LasyInitialization()
        {
            if (_connection == null || _channel == null)
            {
                ConnectionFactory factory = new();

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(RabbitMqNames.ExchangeName, ExchangeType.Direct, true, false);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();

            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}
