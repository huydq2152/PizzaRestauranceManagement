namespace PlantBasedPizza.Order.Core.Services;

public interface ILoyaltyPointService
{
    Task AddLoyaltyPoints(string customerId, string orderIdentifier, decimal orderValue);
}