using MongoDB.Driver;
using PlantBasedPizza.Recipes.Core.Entities;

namespace PlantBasedPizza.Recipes.Infrastructure;

public class RecipeRepository : IRecipeRepository
{
    private readonly IMongoCollection<Recipe> _recipes;

    public RecipeRepository(MongoClient client)
    {
        var database = client.GetDatabase("PlantBasedPizza");
        _recipes = database.GetCollection<Recipe>("recipes");
    }
    
    public async Task<Recipe> Retrieve(string recipeIdentifier)
    {
        var queryBuilder = Builders<Recipe>.Filter.Eq(p => p.RecipeIdentifier, recipeIdentifier);

        var recipe = await _recipes.Find(queryBuilder).FirstOrDefaultAsync();

        return recipe;
    }

    public async Task<IEnumerable<Recipe>> List()
    {
        var recipes = await _recipes.Find(p => true).ToListAsync();

        return recipes;
    }

    public async Task Add(Recipe recipe)
    {
        await _recipes.InsertOneAsync(recipe).ConfigureAwait(false);
    }

    public async Task Update(Recipe recipe)
    {
        var queryBuilder = Builders<Recipe>.Filter.Eq(ord => ord.RecipeIdentifier, recipe.RecipeIdentifier);

        await _recipes.ReplaceOneAsync(
            queryBuilder,
            recipe);
    }
}