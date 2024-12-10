using PlantBasedPizza.Shared.Events;
using PlantBasedPizza.Shared.Logging;
using Saunter.Attributes;

namespace PlantBasedPizza.Events;

[AsyncApi]
public class OrderBakedEvent(string orderIdentifier) : IDomainEvent
{
    public string EventName => "kitchen.baked";
        
    public string EventVersion => "v1";
        
    public string EventId { get; } = Guid.NewGuid().ToString();

    public DateTime EventDate { get; } = DateTime.Now;
    public string CorrelationId { get; set; } = CorrelationContext.GetCorrelationId();

    public string OrderIdentifier { get; private set; } = orderIdentifier;
}