using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders.Application.Models.Options;
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

            builder.Configuration.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("rabbitmq.appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            builder.Services.Configure<RabbitMqConnectionOptions>(builder.Configuration.GetSection("RabbitMqConnectionOptions"));
            builder.Services.Configure<RabbitMqTopologyOptions>(builder.Configuration.GetSection("RabbitMqTopologyOptions"));
            builder.Services.AddRabbitMqMessaging();
            builder.Services.AddHostedService<RabbitMqTopologyDeclare>();

            builder.Services.AddHostedService<OrderCreationRouter>();

            using IHost host = builder.Build();
            await host.RunAsync();
        }
    }
}
