using System.Text.Json.Serialization;
using PlantBasedPizza.Events;

namespace PlantBasedPizza.Delivery.Core.Entities;

public class DeliveryRequest
{
    [JsonConstructor]
    private DeliveryRequest()
    {
    }
        
    public DeliveryRequest(string orderIdentifier, Address deliveryAddress)
    {
        OrderIdentifier = orderIdentifier;
        DeliveryAddress = deliveryAddress;
    }
        
    [JsonPropertyName("orderIdentifier")]
    public string OrderIdentifier { get; private set; } = "";
        
    [JsonPropertyName("driver")]
    public string Driver { get; private set; } = "";
        
    public bool AwaitingCollection => !DriverCollectedOn.HasValue;
        
    [JsonPropertyName("deliveryAddress")]
    public Address DeliveryAddress { get; private set; }

    [JsonPropertyName("driverCollectedOn")]
    public DateTime? DriverCollectedOn { get; private set; }

    [JsonPropertyName("deliveredOn")]
    public DateTime? DeliveredOn { get; private set; }

    public Task ClaimDelivery(string driverName)
    {
        Driver = driverName;
        DriverCollectedOn = DateTime.Now;
        return Task.CompletedTask;
    }

    public Task Deliver()
    {
        DeliveredOn = DateTime.Now;
        return Task.CompletedTask;
    }
}