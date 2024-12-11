using System.Text.Json.Serialization;
using PlantBasedPizza.Events;
using PlantBasedPizza.Recipes.Core.Events;
using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.Recipes.Core.Entities;

public class Recipe
{
    private List<Ingredient> _ingredients;
        
    [JsonConstructor]
    private Recipe()
    {
        RecipeIdentifier = "";
        Name = "";
        _ingredients = new List<Ingredient>();
    }
        
    public Recipe(string recipeIdentifier, string name, decimal price)
    {
        RecipeIdentifier = recipeIdentifier;
        Name = name;
        Price = price;
        _ingredients = new List<Ingredient>();

        DomainEvents.Raise(new RecipeCreatedEvent(this, CorrelationContext.GetCorrelationId()));
    }
        
    [JsonPropertyName("recipeIdentifier")]
    public string RecipeIdentifier { get; private set; }
        
    [JsonPropertyName("name")]
    public string Name { get; private set; }
        
    [JsonPropertyName("price")]
    public decimal Price { get; private set; }

    [JsonPropertyName("ingredients")]
    public IReadOnlyCollection<Ingredient> Ingredients => _ingredients;

    public void AddIngredient(string name, int quantity)
    {
        if (_ingredients == null)
        {
            _ingredients = new List<Ingredient>();
        }
            
        _ingredients.Add(new Ingredient(name, quantity));
    }
}