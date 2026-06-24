namespace Orders.Application.Constants
{
    public class RabbitMqNames
    {
        public const string ExchangeName = "order";
        public const string OrderCreatedQueueName = "order.created";
        public const string PaymentSucceededQueueName = "payment.succeeded";
        public const string PaymentFailedQueueName = "payment.failed";
    }
}
