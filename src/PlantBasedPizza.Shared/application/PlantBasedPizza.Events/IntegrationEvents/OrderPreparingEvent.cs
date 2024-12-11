using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.Events.IntegrationEvents;

public class OrderPreparingEvent(string orderIdentifier) : IDomainEvent
{
    public string EventName => "kitchen.prep-started";
        
    public string EventVersion => "v1";
        
    public string EventId { get; } = Guid.NewGuid().ToString();

    public DateTime EventDate { get; } = DateTime.Now;
    public string CorrelationId { get; set; } = CorrelationContext.GetCorrelationId();

    public string OrderIdentifier { get; private set; } = orderIdentifier;
}