using Microsoft.Extensions.Logging;
using PlantBasedPizza.Events;
using PlantBasedPizza.OrderManager.Core.Entities;
using PlantBasedPizza.Shared.Events;
using Saunter.Attributes;

namespace PlantBasedPizza.OrderManager.Core.Handlers;

[AsyncApi]
public class OrderPreparingEventHandler(IOrderRepository orderRepository, ILogger<OrderPreparingEventHandler> logger)
    : Handles<OrderPreparingEvent>
{
    [Channel("kitchen.prep-started")] // Creates a Channel
    [SubscribeOperation(typeof(OrderPreparingEvent), Summary = "Handle an order prep started event.", OperationId = "kitchen.prep-started")]
    public async Task Handle(OrderPreparingEvent evt)
    {
        logger.LogInformation($"[ORDER-MANAGER] Handling order preparing event");
            
        var order = await orderRepository.Retrieve(evt.OrderIdentifier);

        order.AddHistory("Order prep started");

        await orderRepository.Update(order);
    }
}