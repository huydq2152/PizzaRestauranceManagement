using PlantBasedPizza.Events;
using PlantBasedPizza.OrderManager.Core.Entities;
using PlantBasedPizza.Shared.Events;
using Saunter.Attributes;

namespace PlantBasedPizza.OrderManager.Core.Handlers;

[AsyncApi]
public class OrderBakedEventHandler(IOrderRepository orderRepository) : Handles<OrderBakedEvent>
{
    [Channel("kitchen.baked")] // Creates a Channel
    [SubscribeOperation(typeof(OrderBakedEvent), Summary = "Handle an order baked event.", OperationId = "kitchen.baked")]
    public async Task Handle(OrderBakedEvent evt)
    {
        var order = await orderRepository.Retrieve(evt.OrderIdentifier);

        order.AddHistory("Order baked");

        await orderRepository.Update(order);
    }
}