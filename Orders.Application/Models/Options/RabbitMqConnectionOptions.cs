namespace Orders.Application.Models.Options
{
    public class RabbitMqConnectionOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
    }
}
