using System.Text.Json.Serialization;
using PlantBasedPizza.Order.Core.Entities;

namespace PlantBasedPizza.Order.Core.CreatePickupOrder;

public class CreatePickupOrderCommand
{
    [JsonPropertyName("orderIdentifier")]
    public string OrderIdentifier { get; init; } = "";
        
    [JsonPropertyName("customerIdentifier")]
    public string CustomerIdentifier { get; init; } = "";

    public OrderType OrderType => OrderType.Pickup;
}