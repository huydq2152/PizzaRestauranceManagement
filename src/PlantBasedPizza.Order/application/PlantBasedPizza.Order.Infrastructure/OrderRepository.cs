using System.Diagnostics;
using MongoDB.Driver;
using PlantBasedPizza.Order.Core.Entities;

namespace PlantBasedPizza.Order.Infrastructure;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Core.Entities.Order> _orders;

    public OrderRepository(MongoClient client)
    {
        var database = client.GetDatabase("PlantBasedPizza");
        _orders = database.GetCollection<Core.Entities.Order>("orders");
    }

    public async Task Add(Core.Entities.Order order)
    {
        await _orders.InsertOneAsync(order).ConfigureAwait(false);
    }

    public async Task<Core.Entities.Order> Retrieve(string orderIdentifier)
    {
        var queryBuilder = Builders<Core.Entities.Order>.Filter.Eq(p => p.OrderIdentifier, orderIdentifier);

        var order = await _orders.Find(queryBuilder).FirstOrDefaultAsync().ConfigureAwait(false);

        if (order == null)
        {
            Activity.Current?.AddTag("order.notFound", true);
            throw new OrderNotFoundException(orderIdentifier);
        }

        return order;
    }

    public Task<Core.Entities.Order> Exists(string orderIdentifier)
    {
        var queryBuilder = Builders<Core.Entities.Order>.Filter.Eq(p => p.OrderIdentifier, orderIdentifier);

        return _orders.Find(queryBuilder).FirstOrDefaultAsync();
    }

    public async Task<List<Core.Entities.Order>> GetAwaitingCollection()
    {
        var queryBuilder = Builders<Core.Entities.Order>.Filter.Eq(p => p.OrderType, OrderType.Pickup);

        var order = await _orders.Find(p => p.OrderType == OrderType.Pickup && p.AwaitingCollection).ToListAsync();

        return order;
    }

    public async Task Update(Core.Entities.Order order)
    {
        var queryBuilder = Builders<Core.Entities.Order>.Filter.Eq(ord => ord.OrderIdentifier, order.OrderIdentifier);

        await _orders.ReplaceOneAsync(queryBuilder, order);
    }
}