using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Orders.Infrastructure.RabbitMq
{
    public static class RabbitMqServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services)
        {
            services.AddSingleton(new ConnectionFactory());
            services.AddSingleton(sp =>
            {
                var factory = sp.GetRequiredService<ConnectionFactory>();
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });
            services.AddSingleton(sp =>
            {
                var connection = sp.GetRequiredService<IConnection>();
                return connection.CreateChannelAsync().GetAwaiter().GetResult();
            });

            return services;
        }
    }
}
