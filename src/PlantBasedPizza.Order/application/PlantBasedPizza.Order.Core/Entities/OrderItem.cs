using Newtonsoft.Json;

namespace PlantBasedPizza.Order.Core.Entities;

public class OrderItem
{
    [JsonConstructor]
    private OrderItem()
    {
        RecipeIdentifier = "";
        ItemName = "";
    }
        
    internal OrderItem(string recipeIdentifier, string itemName, int quantity, decimal price)
    {
        RecipeIdentifier = recipeIdentifier;
        ItemName = itemName;
        Quantity = quantity;
        Price = price;
    }
        
    [JsonProperty]
    public string RecipeIdentifier { get; private set; }
        
    [JsonProperty]
    public string ItemName { get; private set; }
        
    [JsonProperty]
    public int Quantity { get; private set; }
        
    [JsonProperty]
    public decimal Price { get; private set; }
}