using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Orders.Worker.IntegrationEvents;

namespace PlantBasedPizza.Orders.Worker.Handlers
{
    public class OrderPreparingEventHandler(
        IOrderRepository orderRepository,
        ILogger<OrderPreparingEventHandler> logger)
    {
        public async Task Handle(OrderPreparingEventV1 evt)
        {
            logger.LogInformation($"[ORDER-MANAGER] Handling order preparing event");
            
            var order = await orderRepository.Retrieve(evt.OrderIdentifier);

            order.AddHistory("Order prep started");

            await orderRepository.Update(order);
        }
    }
}