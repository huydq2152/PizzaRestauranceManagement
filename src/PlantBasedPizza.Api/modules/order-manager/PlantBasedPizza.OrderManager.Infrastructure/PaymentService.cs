using PlantBasedPizza.OrderManager.Core.Entities;
using PlantBasedPizza.OrderManager.Core.Services;

namespace PlantBasedPizza.OrderManager.Infrastructure;

public class PaymentService(Payment.PaymentClient paymentClient) : IPaymentService
{
    public async Task<TakePaymentResult> TakePaymentFor(Order order)
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