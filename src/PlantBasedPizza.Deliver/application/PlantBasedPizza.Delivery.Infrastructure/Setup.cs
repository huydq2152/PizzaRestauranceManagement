using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PlantBasedPizza.Delivery.Core.Entities;
using PlantBasedPizza.Delivery.Core.GetDelivery;
using PlantBasedPizza.Delivery.Core.Handlers;
using PlantBasedPizza.Delivery.Infrastructure.IntegrationEvents;

namespace PlantBasedPizza.Delivery.Infrastructure;

using MongoDB.Bson.Serialization;

public static class Setup
{
    public static IServiceCollection AddDeliveryInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var client = new MongoClient(configuration["DatabaseConnection"]);

        services.AddSingleton(client);
        
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
        services.AddSingleton<IDeliveryEventPublisher, DeliveryEventPublisher>();
        services.AddSingleton<OrderReadyForDeliveryEventHandler>();
        services.AddSingleton<GetDeliveryQueryHandler>();

        return services;
    }
}