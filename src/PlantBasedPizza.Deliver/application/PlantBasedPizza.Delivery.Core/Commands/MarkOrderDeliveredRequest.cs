using System.Text.Json.Serialization;

namespace PlantBasedPizza.Delivery.Core.Commands;

public class MarkOrderDeliveredRequest
{
    [JsonPropertyName("OrderIdentifier")]
    public string OrderIdentifier { get; init; } = "";
}