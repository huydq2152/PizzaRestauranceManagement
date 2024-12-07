using PlantBasedPizza.OrderManager.Core.Services;
using PlantBasedPizza.Recipes.Core.Entities;
using Recipe = PlantBasedPizza.OrderManager.Core.Services.Recipe;

namespace PlantBasedPizza.OrderManager.Infrastructure;

public class RecipeService(IRecipeRepository recipes) : IRecipeService
{
    public async Task<Recipe> GetRecipe(string recipeIdentifier)
    {
        var recipe = await recipes.Retrieve(recipeIdentifier);

        return new Recipe
        {
            Price = recipe.Price,
            ItemName = recipe.Name,
        };
    }
}