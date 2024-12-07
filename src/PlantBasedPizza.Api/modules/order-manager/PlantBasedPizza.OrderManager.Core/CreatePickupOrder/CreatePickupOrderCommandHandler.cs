using PlantBasedPizza.OrderManager.Core.Entities;
using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.OrderManager.Core.CreatePickupOrder;

public class CreatePickupOrderCommandHandler(IOrderRepository orderRepository)
{
    public async Task<OrderDto?> Handle(CreatePickupOrderCommand request)
    {
        try {
            await orderRepository.Retrieve(request.OrderIdentifier);
            
            return null;
        }
        catch (OrderNotFoundException){}
            
        var order = Order.Create(request.OrderIdentifier, request.OrderType, request.CustomerIdentifier, null, CorrelationContext.GetCorrelationId());

        await orderRepository.Add(order);

        return new OrderDto(order);
    }
}