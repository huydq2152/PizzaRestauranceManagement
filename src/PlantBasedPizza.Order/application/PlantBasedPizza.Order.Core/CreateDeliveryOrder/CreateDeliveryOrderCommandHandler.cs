using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.Order.Core.CreateDeliveryOrder;

public class CreateDeliveryOrderCommandHandler(IOrderRepository orderRepository)
{
    public async Task<OrderDto?> Handle(CreateDeliveryOrder request)
    {
        try
        {
            await orderRepository.Retrieve(request.OrderIdentifier);

            return null;
        }
        catch (OrderNotFoundException){}

        var order = Entities.Order.Create(request.OrderIdentifier, request.OrderType, request.CustomerIdentifier,
            new DeliveryDetails
            {
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                AddressLine3 = request.AddressLine3,
                AddressLine4 = request.AddressLine4,
                AddressLine5 = request.AddressLine5,
                Postcode = request.Postcode
            }, CorrelationContext.GetCorrelationId());

        await orderRepository.Add(order);

        return new OrderDto(order);
    }
}