using Microsoft.Extensions.Options;
using Orders.Application.Models;
using Orders.Application.Models.Options;
using Orders.Infrastructure.RabbitMq.AbstractClasses;
using RabbitMQ.Client;

namespace Notifications.Worker.Services
{
    internal class PaymentFailedConsumer(IChannel channel, IOptions<RabbitMqTopologyOptions> options) : BaseRabbitMqConsumer<Message>(channel)
    {
        protected override string QueueName => options.Value.PaymentFailedQueueName;

        protected override Task HandleMessageAsync(Message message)
        {
            Console.WriteLine($"Failed: {message}");
            return Task.CompletedTask;
        }
    }
}
