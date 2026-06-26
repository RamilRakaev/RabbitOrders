using Microsoft.Extensions.Hosting;
using Orders.Application.Constants;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Orders.Infrastructure.RabbitMq.AbstractClasses
{
    public abstract class BaseRabbitMqRouter<TMessage>(IChannel channel, string outpuExchangeName) : BackgroundService
    {
        private readonly IChannel _channel = channel;
        protected readonly string _outpuExchangeName = outpuExchangeName;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AsyncEventingBasicConsumer consumer = new(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                try
                {
                    var message = JsonSerializer.Deserialize<TMessage>(ea.Body.Span);
                    await RouteMessageAsync(message, ea);
                }
                catch (Exception ex)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };
            await _channel.BasicConsumeAsync(RabbitMqNames.OrderCreatedQueueName, false, consumer);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        protected async Task PublishAsync(string queue, ReadOnlyMemory<byte> body)
        {
            BasicProperties properties = new()
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
            };
            await _channel.BasicPublishAsync(_outpuExchangeName, queue, false, properties, body);
        }

        protected abstract Task RouteMessageAsync(TMessage? message, BasicDeliverEventArgs eventArgs);
    }
}
