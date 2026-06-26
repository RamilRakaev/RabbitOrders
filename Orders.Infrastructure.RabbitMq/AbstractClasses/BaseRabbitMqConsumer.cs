using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Orders.Infrastructure.RabbitMq.AbstractClasses
{
    public abstract class BaseRabbitMqConsumer<TMessage>(IChannel channel) : BackgroundService
    {
        private readonly IChannel _channel = channel;

        protected abstract string QueueName { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AsyncEventingBasicConsumer consumer = new(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                try
                {
                    var message = JsonSerializer.Deserialize<TMessage>(ea.Body.Span);
                    await HandleMessageAsync(message);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };
            await _channel.BasicConsumeAsync(QueueName, false, consumer);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        protected abstract Task HandleMessageAsync(TMessage message);
    }
}
