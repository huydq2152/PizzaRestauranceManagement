using System.Text.Json.Serialization;

namespace PlantBasedPizza.LoyaltyPoints.Shared.Core;

public class LoyaltyPointsDTO
{
    public LoyaltyPointsDTO(CustomerLoyaltyPoints points)
    {
        CustomerIdentifier = points.CustomerId;
        TotalPoints = points.TotalPoints;
    }
    
    [JsonPropertyName("customerIdentifier")]
    public string CustomerIdentifier { get; set; }
    
    [JsonPropertyName("totalPoints")]
    public decimal TotalPoints { get; set; }
}