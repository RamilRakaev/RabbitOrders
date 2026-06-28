namespace Orders.Application.Models.Options
{
    public class RabbitMqTopologyOptions
    {
        public string ExchangeName { get; set; }
        public string OrderCreatedQueueName { get; set; }
        public string PaymentSucceededQueueName { get; set; }
        public string PaymentFailedQueueName { get; set; }
    }
}
