using PlantBasedPizza.Events;
using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Order.Infrastructure.IntegrationEvents;
using PlantBasedPizza.Orders.Worker.IntegrationEvents;

namespace PlantBasedPizza.Orders.Worker.Handlers
{
    public class DriverDeliveredOrderEventHandler(IOrderRepository orderRepository, IEventPublisher eventPublisher)
    {
        public async Task Handle(DriverDeliveredOrderEventV1 evt)
        {
            var order = await orderRepository.Retrieve(evt.OrderIdentifier);

            order.CompleteOrder();
            
            await orderRepository.Update(order).ConfigureAwait(false);

            await eventPublisher.Publish(new OrderCompletedIntegrationEventV1
            {
                OrderIdentifier = order.OrderIdentifier,
                CustomerIdentifier = order.CustomerIdentifier,
                OrderValue = order.TotalPrice
            });
        }
    }
}