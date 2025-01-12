using PlantBasedPizza.Order.Core.Services;
using PlantBasedPizza.OrderManager.Infrastructure;

namespace PlantBasedPizza.Order.Infrastructure;

public class PaymentService(Payment.PaymentClient paymentClient) : IPaymentService
{
    public async Task<TakePaymentResult> TakePaymentFor(Core.Entities.Order order)
    {
        var result =
            await paymentClient.TakePaymentAsync(new TakePaymentRequest()
            {
                CustomerIdentifier = order.CustomerIdentifier,
                PaymentAmount = Convert.ToDouble(order.TotalPrice)
            });

        return new TakePaymentResult(result.PaymentStatus, result.IsSuccess);
    }
}