using Microsoft.AspNetCore.Mvc;
using Orders.Api.Services;
using Orders.Application.Models;

namespace Orders.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController(OrderCreationProducer orderCreationProducer, OrderRepository orderRepository) : ControllerBase
    {
        private readonly OrderCreationProducer _orderCreationProducer = orderCreationProducer;
        private readonly OrderRepository _orderRepository = orderRepository;

        [HttpPost("CreateOrder")]
        public async Task<Order> CreateOrder(Order order)
        {
            if (order.Product is not null)
            {
                var createdOrder = await _orderRepository.CreateOrder(order);
                await _orderCreationProducer.PublishSuccessfulCreation(createdOrder);

                return createdOrder;
            }
            else
            {
                await _orderCreationProducer.PublishFailedCreation(order);
                return order;
            }
        }
    }
}
