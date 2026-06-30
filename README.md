# RabbitOrders

Solution:
RabbitOrders.sln

src/
  Orders.Api
  Orders.Application
  Orders.Infrastructure.RabbitMq
  Notifications.Worker
  Payments.Worker


Orders.Api

REST API:
POST /orders


Creates an order and publishes an event:
{
  "eventType": "OrderCreated",
  "orderId": "guid",
  "createdAt": "..."
}


Payments.Worker

Listens to `order.created`, simulates a payment, publishes:
payment.succeeded
payment.failed


Notifications.Worker

Listening to an event and sending a pseudo notification in logs.

What to use

• ASP.NET Core Web API;
• `BackgroundService`;
• DI;
• Options pattern;
• logging;
• RabbitMQ.Client;
• Newtonsoft.Json.
