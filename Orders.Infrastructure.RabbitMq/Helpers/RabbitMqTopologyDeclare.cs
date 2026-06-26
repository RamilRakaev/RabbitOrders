using Orders.Application.Constants;
using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;

namespace Orders.Infrastructure.RabbitMq.Helpers
{
    public class RabbitMqTopologyDeclare(IChannel channel) : IHostedLifecycleService
    {
        private readonly IChannel _channel = channel;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _channel.ExchangeDeclareAsync(RabbitMqNames.ExchangeName, ExchangeType.Direct, true, false);

            await _channel.QueueDeclareAsync(RabbitMqNames.PaymentSucceededQueueName, true, false, false);
            await _channel.QueueBindAsync(RabbitMqNames.PaymentSucceededQueueName, RabbitMqNames.ExchangeName, RabbitMqNames.PaymentSucceededQueueName);

            await _channel.QueueDeclareAsync(RabbitMqNames.PaymentFailedQueueName, true, false, false);
            await _channel.QueueBindAsync(RabbitMqNames.PaymentFailedQueueName, RabbitMqNames.ExchangeName, RabbitMqNames.PaymentFailedQueueName);

            await _channel.QueueDeclareAsync(RabbitMqNames.OrderCreatedQueueName, true, false, false);
            await _channel.QueueBindAsync(RabbitMqNames.OrderCreatedQueueName, RabbitMqNames.ExchangeName, RabbitMqNames.OrderCreatedQueueName);
        }

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
