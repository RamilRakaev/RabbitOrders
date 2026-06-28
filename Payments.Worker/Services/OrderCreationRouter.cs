using Microsoft.Extensions.Options;
using Orders.Application.Models;
using Orders.Application.Models.Options;
using Orders.Infrastructure.RabbitMq.AbstractClasses;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Payments.Worker.Services
{
    internal class OrderCreationRouter(IChannel channel, IOptions<RabbitMqTopologyOptions> options) : BaseRabbitMqRouter<Message>(channel, options)
    {
        private readonly IChannel _channel = channel;
        private readonly RabbitMqTopologyOptions _options = options.Value;

        protected override async Task RouteMessageAsync(Message? message, BasicDeliverEventArgs eventArgs)
        {
            if (message is not null && message.EventType == EventTypes.OrderCreated)
            {
                await PublishAsync(_options.PaymentSucceededQueueName, eventArgs.Body);
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            }
            else
            {
                await PublishAsync(_options.PaymentFailedQueueName, eventArgs.Body);
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
            }
        }
    }
}
