using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlantBasedPizza.Deliver.Core.Entities;
using PlantBasedPizza.Deliver.Core.GetDelivery;
using PlantBasedPizza.Deliver.Core.Handlers;
using PlantBasedPizza.Events;
using PlantBasedPizza.Events.IntegrationEvents;

namespace PlantBasedPizza.Deliver.Infrastructure;

using MongoDB.Bson.Serialization;

public static class Setup
{
    public static IServiceCollection AddDeliveryModuleInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        BsonClassMap.RegisterClassMap<DeliveryRequest>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.SetIgnoreExtraElementsIsInherited(true);
        });
            
        BsonClassMap.RegisterClassMap<Address>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.SetIgnoreExtraElementsIsInherited(true);
        });
            
        services.AddSingleton<IDeliveryRequestRepository, DeliveryRequestRepository>();
        services.AddSingleton<IHandles<OrderReadyForDeliveryEvent>, OrderReadyForDeliveryEventHandler>();
        services.AddSingleton<GetDeliveryQueryHandler>();

        return services;
    }
}