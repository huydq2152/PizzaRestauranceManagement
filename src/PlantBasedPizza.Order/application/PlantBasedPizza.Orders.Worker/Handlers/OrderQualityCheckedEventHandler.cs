using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Orders.Worker.IntegrationEvents;

namespace PlantBasedPizza.Orders.Worker.Handlers
{
    public class OrderQualityCheckedEventHandler(IOrderRepository orderRepository, IOrderEventPublisher eventPublisher)
    {
        public async Task Handle(OrderQualityCheckedEventV1 evt)
        {
            var order = await orderRepository.Retrieve(evt.OrderIdentifier);

            order.AddHistory("Order quality checked");

            if (order.OrderType == OrderType.Delivery)
            {
                order.AddHistory("Sending for delivery");

                await eventPublisher.PublishOrderReadyForDeliveryEventV1(order);
            }
            else
            {
                order.IsAwaitingCollection();
            }

            await orderRepository.Update(order).ConfigureAwait(false);
        }
    }
}