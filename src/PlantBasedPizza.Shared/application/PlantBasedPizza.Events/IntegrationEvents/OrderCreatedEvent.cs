using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.Events.IntegrationEvents;

public class OrderCreatedEvent(string orderIdentifier) : IDomainEvent
{
    private readonly string _eventId = Guid.NewGuid().ToString();

    public string OrderIdentifier { get; private set; } = orderIdentifier;

    public string EventName => "order-manager.order-created";
        
    public string EventVersion => "v1";

    public string EventId => this._eventId;

    public DateTime EventDate { get; } = DateTime.Now;
    public string CorrelationId { get; set; } = CorrelationContext.GetCorrelationId();
}