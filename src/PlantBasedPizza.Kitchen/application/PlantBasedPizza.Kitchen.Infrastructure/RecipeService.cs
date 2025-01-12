using System.Text.Json;
using Microsoft.Extensions.Options;
using PlantBasedPizza.Kitchen.Core.Adapters;
using PlantBasedPizza.Kitchen.Core.Services;

namespace PlantBasedPizza.Kitchen.Infrastructure;

public class RecipeService(IHttpClientFactory clientFactory, IOptions<ServiceEndpoints> endpoints)
    : IRecipeService
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient("service-registry-http-client");
    private readonly ServiceEndpoints _serviceEndpoints = endpoints.Value;

    public async Task<RecipeAdapter> GetRecipe(string recipeIdentifier)
    {
        var recipeResult = await this._httpClient.GetAsync($"{_serviceEndpoints.Recipes}/recipes/{recipeIdentifier}");

        var recipe = JsonSerializer.Deserialize<RecipeAdapter>(await recipeResult.Content.ReadAsStringAsync());

        return recipe;
    }
}