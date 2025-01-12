using PlantBasedPizza.Delivery.Core.Entities;
using PlantBasedPizza.Events;
using Saunter.Attributes;

namespace PlantBasedPizza.Delivery.Infrastructure.IntegrationEvents;

[AsyncApi]
public class DeliveryEventPublisher(IEventPublisher eventPublisher) : IDeliveryEventPublisher
{
    [Channel("delivery.driverCollectedOrder.v1")]
    [PublishOperation(typeof(DriverCollectedOrderEventV1), Summary = "Published when a driver collects an order.")]
    public async Task PublishDriverOrderCollectedEventV1(DeliveryRequest deliveryRequest)
    {
        await eventPublisher.Publish(new DriverCollectedOrderEventV1()
        {
            DriverName = deliveryRequest.Driver,
            OrderIdentifier = deliveryRequest.OrderIdentifier
        });
    }

    [Channel("delivery.driverDeliveredOrder.v1")]
    [PublishOperation(typeof(DriverDeliveredOrderEventV1), Summary = "Published when a driver delivers an order.")]
    public async Task PublishDriverDeliveredOrderEventV1(DeliveryRequest deliveryRequest)
    {
        await eventPublisher.Publish(new DriverDeliveredOrderEventV1()
        {
            OrderIdentifier = deliveryRequest.OrderIdentifier
        });
    }
}