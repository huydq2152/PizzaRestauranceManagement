namespace PlantBasedPizza.Delivery.Core.Entities;

public interface IDeliveryEventPublisher
{
    Task PublishDriverOrderCollectedEventV1(DeliveryRequest deliveryRequest);
    
    Task PublishDriverDeliveredOrderEventV1(DeliveryRequest deliveryRequest);
}