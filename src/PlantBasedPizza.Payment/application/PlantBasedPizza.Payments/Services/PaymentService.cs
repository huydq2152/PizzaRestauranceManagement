using System.Security.Cryptography;
using Grpc.Core;
using PlantBasedPizza.Events;
using PlantBasedPizza.Payments.IntegrationEvents;

namespace PlantBasedPizza.Payments.Services;

public class PaymentService(IEventPublisher eventPublisher) : Payment.PaymentBase
{
    public override async Task<TakePaymentsReply> TakePayment(TakePaymentRequest request, ServerCallContext context)
    {
        var randomSecondDelay = RandomNumberGenerator.GetInt32(1, 250);

        await Task.Delay(TimeSpan.FromSeconds(randomSecondDelay));
        
        await eventPublisher.Publish(new PaymentSuccessfulEvent()
        {
            OrderIdentifier = request.OrderIdentifier,
            CustomerIdentifier = request.CustomerIdentifier
        });

        return new TakePaymentsReply()
        {
            IsSuccess = true,
            PaymentStatus = "SUCCESS"
        };
    }
}