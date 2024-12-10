using PlantBasedPizza.Order.Core.Entities;

namespace PlantBasedPizza.Order.Core.Services;

public interface IPaymentService
{
    Task<TakePaymentResult> TakePaymentFor(Entities.Order order);
}