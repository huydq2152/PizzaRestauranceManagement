using PlantBasedPizza.Kitchen.Core.Adapters;
using PlantBasedPizza.Kitchen.Core.Services;
using PlantBasedPizza.Recipes.Core.Entities;

namespace PlantBasedPizza.Kitchen.Infrastructure;

public class RecipeService(IRecipeRepository recipes) : IRecipeService
{
    public async Task<RecipeAdapter> GetRecipe(string recipeIdentifier)
    {
        var recipe = await recipes.Retrieve(recipeIdentifier);

        return new RecipeAdapter(recipe.RecipeIdentifier);
    }
}