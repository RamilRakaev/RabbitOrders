using Orders.Application.Constants;
using Orders.Application.Models;
using Orders.Infrastructure.RabbitMq.Helpers;

namespace Orders.Api.Services
{
    public class OrderCreationProducer(MessageProducer messageProducer)
    {
        private readonly MessageProducer _messageProducer = messageProducer;

        public async Task PublishSuccessfulCreation(Order createdOrder)
        {
            Message message = new()
            {
                EventType = EventTypes.OrderCreated,
                OrderId = createdOrder.Id,
                CreatedAt = DateTime.UtcNow,
            };
            await _messageProducer.Publish(message, RabbitMqNames.OrderCreatedQueueName);
        }

        public async Task PublishFailedCreation(Order createdOrder)
        {
            Message message = new()
            {
                EventType = EventTypes.OrderFailed,
                CreatedAt = DateTime.UtcNow,
            };
            await _messageProducer.Publish(message, RabbitMqNames.OrderCreatedQueueName);
        }
    }
}
