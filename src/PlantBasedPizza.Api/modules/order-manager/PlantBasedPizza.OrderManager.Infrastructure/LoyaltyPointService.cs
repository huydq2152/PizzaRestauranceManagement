using Microsoft.Extensions.Logging;
using PlantBasedPizza.OrderManager.Core.Services;

namespace PlantBasedPizza.OrderManager.Infrastructure;

public class LoyaltyPointService(Loyalty.LoyaltyClient loyaltyClient, ILogger<LoyaltyPointService> logger)
    : ILoyaltyPointService
{
    public async Task AddLoyaltyPoints(string customerId, string orderIdentifier, decimal orderValue)
    {
        try
        {
            var createLoyaltyPointsResult = await loyaltyClient.AddLoyaltyPointsAsync(
                new AddLoyaltyPointsRequest()
                {
                    CustomerIdentifier = customerId,
                    OrderIdentifier = orderIdentifier,
                    OrderValue = (double)orderValue,
                });

            if (createLoyaltyPointsResult is null)
            {
                throw new Exception("Failure sending loyalty points");
            }
        }
        catch (Exception e)
        {
            logger.LogInformation(e, "Failure");
            throw;
        }
    }
}