using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Orders.Worker.IntegrationEvents;

namespace PlantBasedPizza.Orders.Worker.Handlers
{
    public class OrderBakedEventHandler(IOrderRepository orderRepository)
    {
        public async Task Handle(OrderBakedEventV1 evt)
        {
            var order = await orderRepository.Retrieve(evt.OrderIdentifier);

            order.AddHistory("Order baked");

            await orderRepository.Update(order);
        }
    }
}