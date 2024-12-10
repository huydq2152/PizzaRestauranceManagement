using PlantBasedPizza.Shared.Events;
using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.Events;

public class OrderDeliveredEvent(string orderIdentifier) : IDomainEvent
{
    public string EventName => "delivery.order-delivered";
        
    public string EventVersion => "v1";
        
    public string EventId { get; } = Guid.NewGuid().ToString();

    public DateTime EventDate { get; } = DateTime.Now;
    public string CorrelationId { get; set; } = CorrelationContext.GetCorrelationId();

    public string OrderIdentifier { get; private set; } = orderIdentifier;
}