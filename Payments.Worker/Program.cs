using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Orders.Application.Models;
using Orders.Application.Constants;

namespace Payments.Worker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ConnectionFactory factory = new();
            var connection = await factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(RabbitMqNames.ExchangeName, ExchangeType.Direct, true, false);
            await channel.QueueDeclareAsync(RabbitMqNames.OrderCreatedQueueName, true, false, false);
            await channel.QueueBindAsync(RabbitMqNames.OrderCreatedQueueName, RabbitMqNames.ExchangeName, RabbitMqNames.OrderCreatedQueueName);

            AsyncEventingBasicConsumer consumer = new(channel);
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
                    await channel.BasicPublishAsync(RabbitMqNames.ExchangeName, RabbitMqNames.PaymentSucceededQueueName, false, properties, ea.Body);
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                else
                {
                    await channel.BasicPublishAsync(RabbitMqNames.ExchangeName, RabbitMqNames.PaymentFailedQueueName, false, properties, ea.Body);
                    await channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };
            await channel.BasicConsumeAsync(RabbitMqNames.OrderCreatedQueueName, false, consumer);

            Console.ReadLine();

            await channel.CloseAsync();
            await channel.DisposeAsync();

            await connection.CloseAsync();
            await connection.DisposeAsync();
        }
    }
}
