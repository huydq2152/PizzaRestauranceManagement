namespace PlantBasedPizza.Order.Core.Services;

public interface ILoyaltyPointService
{
    Task<decimal> GetCustomerLoyaltyPoints(string customerId);
}