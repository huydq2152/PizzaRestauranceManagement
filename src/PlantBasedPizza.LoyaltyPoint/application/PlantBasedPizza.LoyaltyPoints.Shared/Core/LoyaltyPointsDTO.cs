using System.Text.Json.Serialization;

namespace PlantBasedPizza.LoyaltyPoints.Shared.Core;

public class LoyaltyPointsDto(CustomerLoyaltyPoints points)
{
    [JsonPropertyName("customerIdentifier")]
    public string CustomerIdentifier { get; set; } = points.CustomerId;

    [JsonPropertyName("totalPoints")]
    public decimal TotalPoints { get; set; } = points.TotalPoints;
}