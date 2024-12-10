namespace PlantBasedPizza.Order.Core.Services;

public interface IRecipeService
{
    Task<Recipe> GetRecipe(string recipeIdentifier);
}