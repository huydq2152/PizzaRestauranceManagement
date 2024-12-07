using PlantBasedPizza.Events;
using PlantBasedPizza.OrderManager.Core.Entities;
using PlantBasedPizza.Shared.Events;
using Saunter.Attributes;

namespace PlantBasedPizza.OrderManager.Core.Handlers;

[AsyncApi("OrderManager")]
public class DriverCollectedOrderEventHandler(IOrderRepository orderRepository) : Handles<DriverCollectedOrderEvent>
{
    [Channel("delivery.driver-collected")] // Creates a Channel
    [SubscribeOperation(typeof(DriverCollectedOrderEvent), Summary = "Handle a driver order collected event.", OperationId = "delivery.driver-collected")]
    public async Task Handle(DriverCollectedOrderEvent evt)
    {
        var order = await orderRepository.Retrieve(evt.OrderIdentifier);

        order.AddHistory($"Order collected by driver {evt.DriverName}");
            
        await orderRepository.Update(order).ConfigureAwait(false);
    }
}