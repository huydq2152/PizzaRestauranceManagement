using System.Diagnostics;
using MongoDB.Driver;
using PlantBasedPizza.Events;
using PlantBasedPizza.LoyaltyPoints.Shared.Core;

namespace PlantBasedPizza.LoyaltyPoints.Shared.Adapters;

public class CustomerLoyaltyPointRepository : ICustomerLoyaltyPointsRepository
{
    private readonly IMongoCollection<CustomerLoyaltyPoints> _loyaltyPoints;
    private readonly IEventPublisher _eventPublisher;

    public CustomerLoyaltyPointRepository(MongoClient client, IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
        var database = client.GetDatabase("LoyaltyPoints");
        _loyaltyPoints = database.GetCollection<CustomerLoyaltyPoints>("loyalty");
    }

    public async Task<CustomerLoyaltyPoints?> GetCurrentPointsFor(string customerIdentifier)
    {
        var queryBuilder = Builders<CustomerLoyaltyPoints>.Filter.Eq(p => p.CustomerId, customerIdentifier);

        var currentPoints = await _loyaltyPoints.Find(queryBuilder).FirstOrDefaultAsync();

        if (currentPoints == null)
        {
            Activity.Current?.AddTag("loyalty.notFoundForCustomer", true);
        }

        return currentPoints;
    }

    public async Task UpdatePoints(CustomerLoyaltyPoints points)
    {
        var queryBuilder = Builders<CustomerLoyaltyPoints>.Filter.Eq(p => p.CustomerId, points.CustomerId);

        var updateDefinition = Builders<CustomerLoyaltyPoints>.Update
            .Set(loyaltyPoint => loyaltyPoint.TotalPoints, points.TotalPoints)
            .Set(loyaltyPoint => loyaltyPoint.History, points.History);

        await _loyaltyPoints.UpdateOneAsync(queryBuilder, updateDefinition, new UpdateOptions { IsUpsert = true });

        await _eventPublisher.Publish(new CustomerLoyaltyPointsUpdatedEvent()
        {
            CustomerIdentifier = points.CustomerId,
            TotalLoyaltyPoints = points.TotalPoints
        });
    }
}