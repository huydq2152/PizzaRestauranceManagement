using Microsoft.Extensions.Logging;
using PlantBasedPizza.Events;
using PlantBasedPizza.Events.IntegrationEvents;
using PlantBasedPizza.Order.Core.Entities;
using Saunter.Attributes;

namespace PlantBasedPizza.Order.Core.Handlers;

[AsyncApi]
public class OrderPrepCompleteEventHandler(
    IOrderRepository orderRepository,
    ILogger<OrderPrepCompleteEventHandler> logger)
    : IHandles<OrderPrepCompleteEvent>
{
    [Channel("kitchen.prep-complete")] // Creates a Channel
    [SubscribeOperation(typeof(OrderPrepCompleteEvent), Summary = "Handle an order prep completed event.", OperationId = "kitchen.prep-complete")]
    public async Task Handle(OrderPrepCompleteEvent evt)
    {
        logger.LogInformation("[ORDER-MANAGER] Handling order prep complete event");
            
        var order = await orderRepository.Retrieve(evt.OrderIdentifier);
            
        logger.LogInformation("[ORDER-MANAGER] Found order");

        order.AddHistory("Order prep completed");
            
        logger.LogInformation("[ORDER-MANAGER] Added history");

        await orderRepository.Update(order);
            
        logger.LogInformation("[ORDER-MANAGER] Wrote updates to database");
    }
}