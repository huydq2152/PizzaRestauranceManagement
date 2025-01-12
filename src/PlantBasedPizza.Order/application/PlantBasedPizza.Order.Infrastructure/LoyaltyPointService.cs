using System.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PlantBasedPizza.Order.Core.Services;
using PlantBasedPizza.OrderManager.Infrastructure;
using StackExchange.Redis;

namespace PlantBasedPizza.Order.Infrastructure;

public class LoyaltyPointService(
    Loyalty.LoyaltyClient loyaltyClient,
    ILogger<LoyaltyPointService> logger,
    IDistributedCache distributedCache)
    : ILoyaltyPointService
{
    public async Task<decimal> GetCustomerLoyaltyPoints(string customerId)
    {
        try
        {
            var cacheCheck = await distributedCache.GetStringAsync(customerId);

            if (cacheCheck != null)
            {
                Activity.Current?.AddTag("loyalty.cacheHit", true);

                return decimal.Parse(cacheCheck);
            }
        }
        catch (RedisServerException ex)
        {
            logger.LogError(ex, "Failure reading loyalty points from cache");

            Activity.Current?.AddTag("cache.failure", true);
        }

        Activity.Current?.AddTag("loyalty.cacheMiss", true);

        var loyaltyPoints = await loyaltyClient.GetCustomerLoyaltyPointsAsync(
            new GetCustomerLoyaltyPointsRequest
            {
                CustomerIdentifier = customerId
            });

        await distributedCache.SetStringAsync(customerId, loyaltyPoints.TotalPoints.ToString("n0"));
        
        return Convert.ToDecimal(loyaltyPoints.TotalPoints);

        return 0;
    }
}