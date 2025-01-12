using System.Text.Json.Serialization;

namespace PlantBasedPizza.Delivery.Core.Commands;

public class AssignDriverRequest
{
    [JsonPropertyName("OrderIdentifier")]
    public string OrderIdentifier { get; init; } = "";
        
    [JsonPropertyName("DriverName")]
    public string DriverName { get; init; } = "";
}