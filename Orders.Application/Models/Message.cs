namespace Orders.Application.Models
{
    public class Message
    {
        public EventTypes EventType { get; set; }
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
