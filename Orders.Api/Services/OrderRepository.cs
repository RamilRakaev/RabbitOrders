using Dapper;
using Microsoft.Data.Sqlite;
using Orders.Application.Models;

namespace Orders.Api.Services
{
    public class OrderRepository : IDisposable
    {
        private readonly SqliteConnection _connection;

        public OrderRepository() => _connection = new("Data Source=Orders.db");

        public async Task<Order> CreateOrder(Order order)
        {
            order.Id = Guid.NewGuid();
            string createOrdersSql =
                @"CREATE TABLE IF NOT EXISTS Orders(
                    Id TEXT(36) PRIMARY KEY,
                    Product TEXT NOT NULL
                );";
            _connection.Execute(createOrdersSql);
            await _connection.ExecuteAsync("INSERT INTO Orders (Product) VALUES (@Product)", new { order.Id, order.Product});

            return order;
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
