using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Orders.Infrastructure.RabbitMq
{
    public abstract class BaseRabbitMqConsumer<TMessage>(IChannel channel) : BackgroundService
    {
        private readonly IChannel _channel = channel;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AsyncEventingBasicConsumer consumer = new(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                BasicProperties properties = new()
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent,
                };
                try
                {
                    await HandleMessageAsync(ea);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };
            await _channel.BasicConsumeAsync(QueueName, false, consumer);
        }

        protected abstract Task HandleMessageAsync(BasicDeliverEventArgs eventArgs);

        protected abstract string QueueName { get; }
    }
}
