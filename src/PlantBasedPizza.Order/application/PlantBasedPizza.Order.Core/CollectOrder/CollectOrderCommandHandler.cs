using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Order.Core.Services;

namespace PlantBasedPizza.Order.Core.CollectOrder;

public class CollectOrderCommandHandler(IOrderRepository orderRepository, ILoyaltyPointService loyaltyPointService)
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
            
            await loyaltyPointService.AddLoyaltyPoints(
                existingOrder.CustomerIdentifier,
                existingOrder.OrderIdentifier,
                existingOrder.TotalPrice);

            await orderRepository.Update(existingOrder).ConfigureAwait(false);

            return new OrderDto(existingOrder);
        }
        catch (OrderNotFoundException)
        {
            return null;
        }
    }
}