using Orders.Api.Services;
using Orders.Application.Models.Options;
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

            builder.Configuration.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("rabbitmq.appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            builder.Services.Configure<RabbitMqConnectionOptions>(builder.Configuration.GetSection("RabbitMqConnectionOptions"));
            builder.Services.Configure<RabbitMqTopologyOptions>(builder.Configuration.GetSection("RabbitMqTopologyOptions"));
            builder.Services.AddRabbitMqMessaging();
            builder.Services.AddHostedService<RabbitMqTopologyDeclare>();

            builder.Services.AddTransient<OrderRepository>();
            builder.Services.AddTransient<MessageProducer>();
            builder.Services.AddTransient<OrderCreationProducer>();

            var app = builder.Build();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
