using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orders.Application.Models.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Orders.Infrastructure.RabbitMq.AbstractClasses
{
    public abstract class BaseRabbitMqRouter<TMessage>(IChannel channel, IOptions<RabbitMqTopologyOptions> options) : BackgroundService
    {
        private readonly IChannel _channel = channel;
        private readonly RabbitMqTopologyOptions _options = options.Value;

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
            await _channel.BasicConsumeAsync(_options.OrderCreatedQueueName, false, consumer);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        protected async Task PublishAsync(string queue, ReadOnlyMemory<byte> body)
        {
            BasicProperties properties = new()
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
            };
            await _channel.BasicPublishAsync(_options.ExchangeName, queue, false, properties, body);
        }

        protected abstract Task RouteMessageAsync(TMessage? message, BasicDeliverEventArgs eventArgs);
    }
}
