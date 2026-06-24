using Orders.Application.Constants;
using Orders.Application.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Notifications.Worker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ConnectionFactory factory = new();
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(RabbitMqNames.ExchangeName, ExchangeType.Direct, true, false);

            await channel.QueueDeclareAsync(RabbitMqNames.PaymentSucceededQueueName, true, false, false);
            await channel.QueueBindAsync(RabbitMqNames.PaymentSucceededQueueName, RabbitMqNames.ExchangeName, RabbitMqNames.PaymentSucceededQueueName);

            await channel.QueueDeclareAsync(RabbitMqNames.PaymentFailedQueueName, true, false, false);
            await channel.QueueBindAsync(RabbitMqNames.PaymentFailedQueueName, RabbitMqNames.ExchangeName, RabbitMqNames.PaymentFailedQueueName);

            AsyncEventingBasicConsumer succeededPaymentConsumer = new(channel);
            succeededPaymentConsumer.ReceivedAsync += async (sender, ea) =>
            {
                var message = JsonSerializer.Deserialize<Message>(ea.Body.Span);
                Console.WriteLine($"Succeeded: {message}");
                await channel.BasicAckAsync(ea.DeliveryTag, false);
            };
            AsyncEventingBasicConsumer failedPaymentConsumer = new(channel);
            failedPaymentConsumer.ReceivedAsync += async (sender, ea) =>
            {
                var message = JsonSerializer.Deserialize<Message>(ea.Body.Span);
                Console.WriteLine($"Failed: {message}");
                await channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await channel.BasicConsumeAsync(RabbitMqNames.PaymentSucceededQueueName, false, succeededPaymentConsumer);
            await channel.BasicConsumeAsync(RabbitMqNames.PaymentFailedQueueName, false, failedPaymentConsumer);

            Console.ReadLine();

            await channel.CloseAsync();
            await channel.DisposeAsync();

            await connection.CloseAsync();
            await connection.DisposeAsync();
        }
    }
}
