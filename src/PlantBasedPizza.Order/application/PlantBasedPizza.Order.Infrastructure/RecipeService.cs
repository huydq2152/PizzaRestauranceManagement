using PlantBasedPizza.Order.Core.Services;
using PlantBasedPizza.Recipes.Core.Entities;
using Recipe = PlantBasedPizza.Order.Core.Services.Recipe;

namespace PlantBasedPizza.Order.Infrastructure;

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