using PlantBasedPizza.Events;
using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Order.Core.Services;
using PlantBasedPizza.Shared.Events;
using Saunter.Attributes;

namespace PlantBasedPizza.Order.Core.Handlers;

[AsyncApi]
public class DriverDeliveredOrderEventHandler(
    IOrderRepository orderRepository,
    ILoyaltyPointService loyaltyPointService)
    : Handles<OrderDeliveredEvent>
{
    [Channel("delivery.order-delivered")] // Creates a Channel
    [SubscribeOperation(typeof(OrderDeliveredEvent), Summary = "Handle an order delivered event.", OperationId = "delivery.order-delivered")]
    public async Task Handle(OrderDeliveredEvent evt)
    {
        var order = await orderRepository.Retrieve(evt.OrderIdentifier);

        order.CompleteOrder();
            
        await orderRepository.Update(order).ConfigureAwait(false);
        await loyaltyPointService.AddLoyaltyPoints(order.CustomerIdentifier, evt.OrderIdentifier, order.TotalPrice);
    }
}