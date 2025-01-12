using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Orders.Worker.IntegrationEvents;

namespace PlantBasedPizza.Orders.Worker.Handlers
{
    public class OrderPrepCompleteEventHandler(
        IOrderRepository orderRepository,
        ILogger<OrderPrepCompleteEventHandler> logger)
    {
        public async Task Handle(OrderPrepCompleteEventV1 evt)
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
}