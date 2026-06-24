using Microsoft.AspNetCore.Mvc;
using Orders.Api.Services;
using Orders.Application.Models;

namespace Orders.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        [HttpPost("CreateOrder")]
        public async Task<Order> CreateOrder(Order order)
        {
            await using OrderCreationProducer orderCreationProducer = new();
            if (order.Product is not null)
            {

                using OrderRepository repository = new();
                var createdOrder = await repository.CreateOrder(order);

                await orderCreationProducer.PublishSuccessfulCreation(createdOrder);

                return createdOrder;
            }
            else
            {
                await orderCreationProducer.PublishFailedCreation(order);
                return order;
            }
        }
    }
}
