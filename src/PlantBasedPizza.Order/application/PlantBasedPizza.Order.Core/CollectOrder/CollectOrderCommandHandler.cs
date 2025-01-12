using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Order.Core.Services;

namespace PlantBasedPizza.Order.Core.CollectOrder;

public class CollectOrderCommandHandler(IOrderRepository orderRepository, IOrderEventPublisher eventPublisher)
{
    public async Task<OrderDto?> Handle(CollectOrderRequest command)
    {
        try
        {
            var existingOrder = await orderRepository.Retrieve(command.OrderIdentifier);
            
            if (existingOrder.OrderType == OrderType.Delivery || !existingOrder.AwaitingCollection)
            {
                return new OrderDto(existingOrder);
            }

            existingOrder.CompleteOrder();
            
            await eventPublisher.PublishOrderCompletedEventV1(existingOrder);

            await orderRepository.Update(existingOrder).ConfigureAwait(false);

            return new OrderDto(existingOrder);
        }
        catch (OrderNotFoundException)
        {
            return null;
        }
    }
}