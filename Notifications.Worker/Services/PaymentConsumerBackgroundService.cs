using Microsoft.Extensions.Hosting;
using Orders.Application.Constants;
using Orders.Application.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Notifications.Worker.Services
{
    internal class PaymentConsumerBackgroundService(IChannel channel) : BackgroundService
    {
        private readonly IChannel _channel = channel;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AsyncEventingBasicConsumer succeededPaymentConsumer = new(_channel);
            succeededPaymentConsumer.ReceivedAsync += async (sender, ea) =>
            {
                var message = JsonSerializer.Deserialize<Message>(ea.Body.Span);
                Console.WriteLine($"Succeeded: {message}");
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };
            AsyncEventingBasicConsumer failedPaymentConsumer = new(_channel);
            failedPaymentConsumer.ReceivedAsync += async (sender, ea) =>
            {
                var message = JsonSerializer.Deserialize<Message>(ea.Body.Span);
                Console.WriteLine($"Failed: {message}");
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(RabbitMqNames.PaymentSucceededQueueName, false, succeededPaymentConsumer);
            await _channel.BasicConsumeAsync(RabbitMqNames.PaymentFailedQueueName, false, failedPaymentConsumer);
        }
    }
}
