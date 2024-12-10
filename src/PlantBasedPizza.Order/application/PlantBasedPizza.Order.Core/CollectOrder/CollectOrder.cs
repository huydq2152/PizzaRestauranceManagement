using System.Text.Json.Serialization;

namespace PlantBasedPizza.Order.Core.CollectOrder;

public class CollectOrderRequest
{
    [JsonPropertyName("OrderIdentifier")]
    public string OrderIdentifier => "";
}