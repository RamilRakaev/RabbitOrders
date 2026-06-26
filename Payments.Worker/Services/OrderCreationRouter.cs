using Microsoft.Extensions.Hosting;
using Orders.Application.Constants;
using Orders.Application.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Payments.Worker.Services
{
    internal class OrderCreationRouter(IChannel channel) : BackgroundService
    {
        private readonly IChannel _channel = channel;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AsyncEventingBasicConsumer consumer = new(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var message = JsonSerializer.Deserialize<Message>(ea.Body.Span);
                BasicProperties properties = new()
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent,
                };
                if (message is not null && message.EventType == EventTypes.OrderCreated)
                {
                    await _channel.BasicPublishAsync(RabbitMqNames.ExchangeName, RabbitMqNames.PaymentSucceededQueueName, false, properties, ea.Body);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                else
                {
                    await _channel.BasicPublishAsync(RabbitMqNames.ExchangeName, RabbitMqNames.PaymentFailedQueueName, false, properties, ea.Body);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };
            await _channel.BasicConsumeAsync(RabbitMqNames.OrderCreatedQueueName, false, consumer);
        }
    }
}
