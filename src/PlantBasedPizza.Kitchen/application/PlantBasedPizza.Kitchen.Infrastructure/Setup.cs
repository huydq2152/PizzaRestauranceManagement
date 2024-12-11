using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlantBasedPizza.Events;
using PlantBasedPizza.Events.IntegrationEvents;
using PlantBasedPizza.Kitchen.Core.Entities;
using PlantBasedPizza.Kitchen.Core.Handlers;
using PlantBasedPizza.Kitchen.Core.Services;

namespace PlantBasedPizza.Kitchen.Infrastructure;

using MongoDB.Bson.Serialization;

public static class Setup
{
    public static IServiceCollection AddKitchenInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        BsonClassMap.RegisterClassMap<KitchenRequest>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.SetIgnoreExtraElementsIsInherited(true);
        });
            
        services.AddSingleton<IRecipeService, RecipeService>();
        services.AddSingleton<IOrderManagerService, OrderManagerService>();
        services.AddSingleton<IHandles<OrderSubmittedEvent>, OrderSubmittedEventHandler>();
        services.AddSingleton<IKitchenRequestRepository, KitchenRequestRepository>();

        return services;
    }
}