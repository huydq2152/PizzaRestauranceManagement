using System.Text.Json;
using Microsoft.Extensions.Options;
using PlantBasedPizza.Order.Core.Services;
using Recipe = PlantBasedPizza.Order.Core.Services.Recipe;

namespace PlantBasedPizza.Order.Infrastructure;

public class RecipeService(IHttpClientFactory clientFactory, IOptions<ServiceEndpoints> endpoints)
    : IRecipeService
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient("service-registry-http-client");
    private readonly ServiceEndpoints _serviceEndpoints = endpoints.Value;

    public async Task<Recipe> GetRecipe(string recipeIdentifier)
    {
        var recipeResult = await _httpClient.GetAsync($"{_serviceEndpoints.Recipes}/recipes/{recipeIdentifier}");

        var recipe = JsonSerializer.Deserialize<Recipe>(await recipeResult.Content.ReadAsStringAsync());

        return recipe;
    }
}