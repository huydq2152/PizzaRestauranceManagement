using System.Text.Json.Serialization;

namespace PlantBasedPizza.Kitchen.Core.Adapters;

public class RecipeAdapter
{
    [JsonConstructor]
    private RecipeAdapter()
    {
    }
        
    public RecipeAdapter(string recipeIdentifier)
    {
        RecipeIdentifier = recipeIdentifier;
    }
        
    [JsonPropertyName("recipeIdentifier")]
    public string RecipeIdentifier { get; set; } = "";
}