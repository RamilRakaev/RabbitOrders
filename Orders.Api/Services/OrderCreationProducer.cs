using Microsoft.Extensions.Options;
using Orders.Application.Models;
using Orders.Application.Models.Options;
using Orders.Infrastructure.RabbitMq.Helpers;

namespace Orders.Api.Services
{
    public class OrderCreationProducer(MessageProducer messageProducer, IOptions<RabbitMqTopologyOptions> options)
    {
        private readonly MessageProducer _messageProducer = messageProducer;
        private readonly RabbitMqTopologyOptions _options = options.Value;

        public async Task PublishSuccessfulCreation(Order createdOrder)
        {
            Message message = new()
            {
                EventType = EventTypes.OrderCreated,
                OrderId = createdOrder.Id,
                CreatedAt = DateTime.UtcNow,
            };
            await _messageProducer.Publish(message, _options.OrderCreatedQueueName);
        }

        public async Task PublishFailedCreation(Order createdOrder)
        {
            Message message = new()
            {
                EventType = EventTypes.OrderFailed,
                CreatedAt = DateTime.UtcNow,
            };
            await _messageProducer.Publish(message, _options.OrderCreatedQueueName);
        }
    }
}
