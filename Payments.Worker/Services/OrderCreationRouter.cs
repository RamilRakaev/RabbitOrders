using Orders.Application.Constants;
using Orders.Application.Models;
using Orders.Infrastructure.RabbitMq.AbstractClasses;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Payments.Worker.Services
{
    internal class OrderCreationRouter(IChannel channel) : BaseRabbitMqRouter<Message>(channel, RabbitMqNames.ExchangeName)
    {
        private readonly IChannel _channel = channel;

        protected override async Task RouteMessageAsync(Message? message, BasicDeliverEventArgs eventArgs)
        {
            if (message is not null && message.EventType == EventTypes.OrderCreated)
            {
                await PublishAsync(RabbitMqNames.PaymentSucceededQueueName, eventArgs.Body);
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            }
            else
            {
                await PublishAsync(RabbitMqNames.PaymentFailedQueueName, eventArgs.Body);
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
            }
        }
    }
}
