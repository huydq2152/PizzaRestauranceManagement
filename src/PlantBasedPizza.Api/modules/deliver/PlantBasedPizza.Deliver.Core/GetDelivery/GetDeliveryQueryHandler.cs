using System.Diagnostics;
using PlantBasedPizza.Deliver.Core.Entities;

namespace PlantBasedPizza.Deliver.Core.GetDelivery;

public class GetDeliveryQueryHandler(IDeliveryRequestRepository deliveryRequestRepository)
{
    public async Task<DeliveryRequestDto?> Handle(GetDeliveryQuery query)
    {
        Activity.Current?.AddTag("orderIdentifier", query.OrderIdentifier);
            
        var deliveryRequest = await deliveryRequestRepository.GetDeliveryStatusForOrder(query.OrderIdentifier);

        return deliveryRequest != null ? new DeliveryRequestDto(deliveryRequest) : null;
    }
}