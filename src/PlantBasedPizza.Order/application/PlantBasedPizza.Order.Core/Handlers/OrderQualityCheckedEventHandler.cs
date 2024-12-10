using PlantBasedPizza.Events;
using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Shared.Events;
using Saunter.Attributes;

namespace PlantBasedPizza.Order.Core.Handlers;

[AsyncApi]
public class OrderQualityCheckedEventHandler(IOrderRepository orderRepository) : Handles<OrderQualityCheckedEvent>
{
    [Channel("kitchen.quality-checked")] // Creates a Channel
    [SubscribeOperation(typeof(OrderQualityCheckedEvent), Summary = "Handle an order quality event.", OperationId = "kitchen.quality-checked")]
    public async Task Handle(OrderQualityCheckedEvent evt)
    {
        var order = await orderRepository.Retrieve(evt.OrderIdentifier);

        order.AddHistory("Order quality checked");

        if (order.OrderType == OrderType.Delivery)
        {
            order.AddHistory("Sending for delivery");

            await DomainEvents.Raise(new OrderReadyForDeliveryEvent(order.OrderIdentifier,
                order.DeliveryDetails.AddressLine1, order.DeliveryDetails.AddressLine2,
                order.DeliveryDetails.AddressLine3, order.DeliveryDetails.AddressLine4,
                order.DeliveryDetails.AddressLine5, order.DeliveryDetails.Postcode));
        }
        else
        {
            order.IsAwaitingCollection();
        }

        await orderRepository.Update(order).ConfigureAwait(false);
    }
}