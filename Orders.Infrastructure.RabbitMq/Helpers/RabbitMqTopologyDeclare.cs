using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orders.Application.Models.Options;
using RabbitMQ.Client;

namespace Orders.Infrastructure.RabbitMq.Helpers
{
    public class RabbitMqTopologyDeclare(IChannel channel, IOptions<RabbitMqTopologyOptions> options) : IHostedLifecycleService
    {
        private readonly IChannel _channel = channel;
        private readonly RabbitMqTopologyOptions _options = options.Value;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Direct, true, false);

            await _channel.QueueDeclareAsync(_options.PaymentSucceededQueueName, true, false, false);
            await _channel.QueueBindAsync(_options.PaymentSucceededQueueName, _options.ExchangeName, _options.PaymentSucceededQueueName);

            await _channel.QueueDeclareAsync(_options.PaymentFailedQueueName, true, false, false);
            await _channel.QueueBindAsync(_options.PaymentFailedQueueName, _options.ExchangeName, _options.PaymentFailedQueueName);

            await _channel.QueueDeclareAsync(_options.OrderCreatedQueueName, true, false, false);
            await _channel.QueueBindAsync(_options.OrderCreatedQueueName, _options.ExchangeName, _options.OrderCreatedQueueName);
        }

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
