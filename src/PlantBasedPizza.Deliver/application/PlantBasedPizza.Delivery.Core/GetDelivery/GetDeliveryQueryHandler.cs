using System.Diagnostics;
using PlantBasedPizza.Delivery.Core.Entities;

namespace PlantBasedPizza.Delivery.Core.GetDelivery;

public class GetDeliveryQueryHandler(IDeliveryRequestRepository deliveryRequestRepository)
{
    public async Task<DeliveryRequestDto?> Handle(GetDeliveryQuery query)
    {
        Activity.Current?.AddTag("orderIdentifier", query.OrderIdentifier);
            
        var deliveryRequest = await deliveryRequestRepository.GetDeliveryStatusForOrder(query.OrderIdentifier);

        return deliveryRequest != null ? new DeliveryRequestDto(deliveryRequest) : null;
    }
}