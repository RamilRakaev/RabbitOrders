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

                Console.WriteLine("RabbitMQ connection settings:");
                Console.WriteLine($"HostName: {connectionOptions.HostName}");
                Console.WriteLine($"Port: {connectionOptions.Port}");
                Console.WriteLine($"UserName: {connectionOptions.UserName}");

                return new ConnectionFactory()
                {
                    UserName = connectionOptions.UserName,
                    Password = connectionOptions.Password,
                    HostName = connectionOptions.HostName,
                    Port = connectionOptions.Port,
                };
            });
            services.AddSingleton<IConnection>(sp =>
            {
                var factory = sp.GetRequiredService<ConnectionFactory>();

                const int maxRetries = 10;

                for (var attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        Console.WriteLine($"Connecting to RabbitMQ. Attempt {attempt}/{maxRetries}");
                        Console.WriteLine($"Host: {factory.HostName}, Port: {factory.Port}, User: {factory.UserName}");

                        return factory.CreateConnectionAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"RabbitMQ connection failed. Attempt {attempt}/{maxRetries}");
                        Console.WriteLine(ex.ToString());

                        if (attempt >= maxRetries)
                            throw;

                        Thread.Sleep(TimeSpan.FromSeconds(3));
                    }
                }

                throw new InvalidOperationException("Could not connect to RabbitMQ.");
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
