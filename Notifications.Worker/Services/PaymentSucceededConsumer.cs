using Orders.Application.Constants;
using Orders.Application.Models;
using Orders.Infrastructure.RabbitMq.AbstractClasses;
using RabbitMQ.Client;

namespace Notifications.Worker.Services
{
    internal class PaymentSucceededConsumer(IChannel channel) : BaseRabbitMqConsumer<Message>(channel)
    {
        protected override string QueueName => RabbitMqNames.PaymentSucceededQueueName;

        protected override Task HandleMessageAsync(Message message)
        {
            Console.WriteLine($"Succeeded: {message}");
            return Task.CompletedTask;
        }
    }
}
