using PlantBasedPizza.Events;

namespace PlantBasedPizza.Payments.IntegrationEvents;

public class PaymentSuccessfulEvent : IntegrationEvent
{
    public override string EventName => "payments.paymentSuccessful";
    public override string EventVersion => "v1";
    
    public override Uri Source => new("https://payments.plantbasedpizza.com");
    
    public string CustomerIdentifier { get; init; }
    public string OrderIdentifier { get; init; }
}