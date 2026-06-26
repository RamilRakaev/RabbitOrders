using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Worker.Services;
using Orders.Infrastructure.RabbitMq;
using Orders.Infrastructure.RabbitMq.Helpers;

namespace Notifications.Worker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddRabbitMqMessaging();
            builder.Services.AddHostedService<RabbitMqTopologyDeclare>();
            builder.Services.AddHostedService<PaymentSucceededConsumer>();
            builder.Services.AddHostedService<PaymentFailedConsumer>();

            using IHost host = builder.Build();

            await host.RunAsync();
        }
    }
}
