using Orders.Api.Services;
using Orders.Infrastructure.RabbitMq;
using Orders.Infrastructure.RabbitMq.Helpers;

namespace Orders.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddRabbitMqMessaging();
            builder.Services.AddTransient<MessageProducer>();
            builder.Services.AddTransient<OrderCreationProducer>();
            builder.Services.AddHostedService<RabbitMqTopologyDeclare>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
