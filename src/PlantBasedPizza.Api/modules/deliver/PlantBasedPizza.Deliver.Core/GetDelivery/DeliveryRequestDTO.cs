
using PlantBasedPizza.Deliver.Core.Entities;

namespace PlantBasedPizza.Deliver.Core.GetDelivery;

public class DeliveryRequestDto(DeliveryRequest request)
{
    public string OrderIdentifier { get; set; } = request.OrderIdentifier;

    public string Driver { get; set; } = request.Driver;

    public bool AwaitingCollection { get; set; } = request.AwaitingCollection;

    public AddressDto? DeliveryAddress { get; set; } = new(request.DeliveryAddress);

    public DateTime? DriverCollectedOn { get; set; } = request.DriverCollectedOn;

    public DateTime? DeliveredOn { get; set; } = request.DeliveredOn;
}