using PlantBasedPizza.Events;
using PlantBasedPizza.Order.Core.Entities;
using Saunter.Attributes;

namespace PlantBasedPizza.Order.Infrastructure.IntegrationEvents;

[AsyncApi]
public class OrderEventPublisher(IEventPublisher eventPublisher) : IOrderEventPublisher
{
    [Channel("order.orderCompleted.v1")]
    [PublishOperation(typeof(OrderCompletedIntegrationEventV1), Summary = "Published when an order is completed.")]
    public async Task PublishOrderCompletedEventV1(Core.Entities.Order order)
    {
        await eventPublisher.Publish(new OrderCompletedIntegrationEventV1
        {
            OrderIdentifier = order.OrderIdentifier,
            CustomerIdentifier = order.CustomerIdentifier,
            OrderValue = order.TotalPrice
        });
    }

    [Channel("order.readyForDelivery.v1")]
    [PublishOperation(typeof(OrderReadyForDeliveryEventV1), Summary = "Published when a delivery order is ready for delivery.")]
    public async Task PublishOrderReadyForDeliveryEventV1(Core.Entities.Order order)
    {
        await eventPublisher.Publish(new OrderReadyForDeliveryEventV1
        {
            OrderIdentifier = order.OrderIdentifier,
            DeliveryAddressLine1 = order.DeliveryDetails.AddressLine1,
            DeliveryAddressLine2 = order.DeliveryDetails.AddressLine2,
            DeliveryAddressLine3 = order.DeliveryDetails.AddressLine3,
            DeliveryAddressLine4 = order.DeliveryDetails.AddressLine4,
            DeliveryAddressLine5 = order.DeliveryDetails.AddressLine5,
            Postcode = order.DeliveryDetails.Postcode,
        });
    }

    [Channel("order.orderSubmitted.v1")]
    [PublishOperation(typeof(OrderSubmittedEventV1), Summary = "Published when an order is submitted and paid for.")]
    public async Task PublishOrderSubmittedEventV1(Core.Entities.Order order)
    {
        await eventPublisher.Publish(new OrderSubmittedEventV1
        {
            OrderIdentifier = order.OrderIdentifier,
            Items = order.Items.Select(item => new OrderSubmittedEventItem
            {
                ItemName = item.ItemName,
                RecipeIdentifier = item.RecipeIdentifier
            }).ToList()
        });
    }
}