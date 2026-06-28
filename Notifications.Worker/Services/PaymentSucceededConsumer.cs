using Microsoft.Extensions.Options;
using Orders.Application.Models;
using Orders.Application.Models.Options;
using Orders.Infrastructure.RabbitMq.AbstractClasses;
using RabbitMQ.Client;

namespace Notifications.Worker.Services
{
    internal class PaymentSucceededConsumer(IChannel channel, IOptions<RabbitMqTopologyOptions> options) : BaseRabbitMqConsumer<Message>(channel)
    {
        protected override string QueueName => options.Value.PaymentSucceededQueueName;

        protected override Task HandleMessageAsync(Message message)
        {
            Console.WriteLine($"Succeeded: {message.OrderId} {message.EventType} {message.CreatedAt}");
            return Task.CompletedTask;
        }
    }
}
