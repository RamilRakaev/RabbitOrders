using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orders.Application.Models.Options;
using RabbitMQ.Client;

namespace Orders.Infrastructure.RabbitMq
{
    public static class RabbitMqServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var connectionOptions = sp.GetRequiredService<IOptions<RabbitMqConnectionOptions>>().Value;
                return new ConnectionFactory()
                {
                    UserName = connectionOptions.UserName,
                    Password = connectionOptions.Password,
                    HostName = connectionOptions.HostName,
                    Port = connectionOptions.Port,
                };
            });
            services.AddSingleton(sp =>
            {
                var factory = sp.GetRequiredService<ConnectionFactory>();
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });
            services.AddTransient(sp =>
            {
                var connection = sp.GetRequiredService<IConnection>();
                return connection.CreateChannelAsync().GetAwaiter().GetResult();
            });

            return services;
        }
    }
}
