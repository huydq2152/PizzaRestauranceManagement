using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.Order.Core.CreatePickupOrder;

public class CreatePickupOrderCommandHandler(IOrderRepository orderRepository)
{
    public async Task<OrderDto?> Handle(CreatePickupOrderCommand request)
    {
        try {
            await orderRepository.Retrieve(request.OrderIdentifier);
            
            return null;
        }
        catch (OrderNotFoundException){}
            
        var order = Entities.Order.Create(request.OrderIdentifier, request.OrderType, request.CustomerIdentifier, null, CorrelationContext.GetCorrelationId());

        await orderRepository.Add(order);

        return new OrderDto(order);
    }
}