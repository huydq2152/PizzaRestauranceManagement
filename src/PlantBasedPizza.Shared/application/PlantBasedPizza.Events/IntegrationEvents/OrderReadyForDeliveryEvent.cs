using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.Events.IntegrationEvents
{
    public class OrderReadyForDeliveryEvent(
        string orderIdentifier,
        string addressLine1,
        string addressLine2,
        string addressLine3,
        string addressLine4,
        string addressLine5,
        string postcode)
        : IDomainEvent
    {
        public string EventName => "order-manager.ready-for-delivery";
        public string EventVersion => "v1";

        public string EventId { get; } = Guid.NewGuid().ToString();

        public DateTime EventDate { get; } = DateTime.Now;
        public string CorrelationId { get; set; } = CorrelationContext.GetCorrelationId();

        public string OrderIdentifier { get; private set; } = orderIdentifier;

        public string DeliveryAddressLine1 { get; private set; } = addressLine1;

        public string DeliveryAddressLine2 { get; private set; } = addressLine2;

        public string DeliveryAddressLine3 { get; private set; } = addressLine3;

        public string DeliveryAddressLine4 { get; private set; } = addressLine4;

        public string DeliveryAddressLine5 { get; private set; } = addressLine5;

        public string Postcode { get; private set; } = postcode;
    }
}