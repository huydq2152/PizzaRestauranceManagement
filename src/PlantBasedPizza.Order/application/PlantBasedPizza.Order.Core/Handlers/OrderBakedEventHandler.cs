using PlantBasedPizza.Events;
using PlantBasedPizza.Events.IntegrationEvents;
using PlantBasedPizza.Order.Core.Entities;
using Saunter.Attributes;

namespace PlantBasedPizza.Order.Core.Handlers;

[AsyncApi]
public class OrderBakedEventHandler(IOrderRepository orderRepository) : IHandles<OrderBakedEvent>
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