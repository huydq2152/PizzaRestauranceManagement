using Newtonsoft.Json;

namespace PlantBasedPizza.Order.Core.Entities;

public class DeliveryDetails
{
    [JsonConstructor]
    public DeliveryDetails()
    {
        AddressLine1 = "";
        AddressLine2 = "";
        AddressLine3 = "";
        AddressLine4 = "";
        AddressLine5 = "";
        Postcode = "";
    }
        
    [JsonProperty]
    public string AddressLine1 { get; init; }
        
    [JsonProperty]
    public string AddressLine2 { get; init; }
        
    [JsonProperty]
    public string AddressLine3 { get; init; }
        
    [JsonProperty]
    public string AddressLine4 { get; init; }
        
    [JsonProperty]
    public string AddressLine5 { get; init; }
        
    [JsonProperty]
    public string Postcode { get; init; }
}