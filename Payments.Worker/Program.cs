using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders.Infrastructure.RabbitMq;
using Orders.Infrastructure.RabbitMq.Helpers;
using Payments.Worker.Services;

namespace Payments.Worker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddRabbitMqMessaging();
            builder.Services.AddHostedService<RabbitMqTopologyDeclare>();
            builder.Services.AddHostedService<OrderCreationRouter>();

            using IHost host = builder.Build();
            await host.RunAsync();
        }
    }
}
