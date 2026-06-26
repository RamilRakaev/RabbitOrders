using Orders.Application.Constants;
using Orders.Application.Models;
using Orders.Infrastructure.RabbitMq.AbstractClasses;
using RabbitMQ.Client;

namespace Notifications.Worker.Services
{
    internal class PaymentFailedConsumer(IChannel channel) : BaseRabbitMqConsumer<Message>(channel)
    {
        protected override string QueueName => RabbitMqNames.PaymentFailedQueueName;

        protected override Task HandleMessageAsync(Message message)
        {
            Console.WriteLine($"Failed: {message}");
            return Task.CompletedTask;
        }
    }
}
