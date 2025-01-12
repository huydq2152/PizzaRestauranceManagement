using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Orders.Worker.IntegrationEvents;

namespace PlantBasedPizza.Orders.Worker.Handlers
{
    public class DriverCollectedOrderEventHandler(IOrderRepository orderRepository)
    {
        public async Task Handle(DriverCollectedOrderEventV1 evt)
        {
            var order = await orderRepository.Retrieve(evt.OrderIdentifier);

            order.AddHistory($"Order collected by driver {evt.DriverName}");
            
            await orderRepository.Update(order).ConfigureAwait(false);
        }
    }
}