using PlantBasedPizza.Shared.Events;
using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.Events;

public class OrderQualityCheckedEvent(string orderIdentifier) : IDomainEvent
{
    public string EventName => "kitchen.quality-checked";
        
    public string EventVersion => "v1";
        
    public string EventId { get; } = Guid.NewGuid().ToString();

    public DateTime EventDate { get; } = DateTime.Now;
    public string CorrelationId { get; set; } = CorrelationContext.GetCorrelationId();

    public string OrderIdentifier { get; private set; } = orderIdentifier;
}