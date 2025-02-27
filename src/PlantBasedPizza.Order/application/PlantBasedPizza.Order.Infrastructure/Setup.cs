using Grpc.Core;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PlantBasedPizza.Events;
using PlantBasedPizza.Order.Core.AddItemToOrder;
using PlantBasedPizza.Order.Core.CollectOrder;
using PlantBasedPizza.Order.Core.CreateDeliveryOrder;
using PlantBasedPizza.Order.Core.CreatePickupOrder;
using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Order.Core.Services;
using PlantBasedPizza.Order.Infrastructure.IntegrationEvents;
using PlantBasedPizza.OrderManager.Infrastructure;
using PlantBasedPizza.Shared.ServiceDiscovery;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace PlantBasedPizza.Order.Infrastructure;

using MongoDB.Bson.Serialization;

public static class Setup
{
    public static IServiceCollection AddOrderManagerInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ServiceEndpoints>(configuration.GetSection("Services"));
            
        var client = new MongoClient(configuration["DatabaseConnection"]);

        services.AddSingleton(client);
        
        BsonClassMap.RegisterClassMap<Core.Entities.Order>(map =>
        {
            map.AutoMap();
            map.MapField("_items");
            map.MapField("_history");
            map.SetIgnoreExtraElements(true);
            map.SetIgnoreExtraElementsIsInherited(true);
        });

        BsonClassMap.RegisterClassMap<OrderItem>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.SetIgnoreExtraElementsIsInherited(true);
        });

        BsonClassMap.RegisterClassMap<DeliveryDetails>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.SetIgnoreExtraElementsIsInherited(true);
        });

        services.AddGrpcClient(configuration);
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["RedisConnectionString"];
            options.InstanceName = "Orders";
        });

        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<CollectOrderCommandHandler>();
        services.AddSingleton<AddItemToOrderHandler>();
        services.AddSingleton<CreateDeliveryOrderCommandHandler>();
        services.AddSingleton<CreatePickupOrderCommandHandler>();
        services.AddSingleton<IRecipeService, RecipeService>();
        services.AddSingleton<ILoyaltyPointService, LoyaltyPointService>();
        services.AddSingleton<IPaymentService, PaymentService>();
        services.AddSingleton<OrderManagerHealthChecks>();
        services.AddSingleton<IOrderEventPublisher, OrderEventPublisher>();

        services.AddHttpClient("service-registry-http-client")
            .AddHttpMessageHandler<ServiceRegistryHttpMessageHandler>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(GetRetryPolicy());

        services.AddLogging();

        return services;
    }

    private static IServiceCollection AddGrpcClient(this IServiceCollection services, IConfiguration configuration)
    {
        var defaultMethodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };

        services.AddGrpcClient<Loyalty.LoyaltyClient>(o =>
            {
                o.Address = new Uri(configuration["Services:LoyaltyInternal"]);
            })
            .ConfigureChannel((provider, channel) =>
            {
                channel.HttpHandler = provider.GetRequiredService<ServiceRegistryHttpMessageHandler>();
                channel.ServiceConfig = new ServiceConfig() { MethodConfigs = { defaultMethodConfig } };
            });

        services.AddGrpcClient<Payment.PaymentClient>(o =>
            {
                o.Address = new Uri(configuration["Services:PaymentInternal"]);
            })
            .ConfigureChannel((provider, channel) =>
            {
                channel.HttpHandler = provider.GetRequiredService<ServiceRegistryHttpMessageHandler>();
                channel.ServiceConfig = new ServiceConfig() { MethodConfigs = { defaultMethodConfig } };
            });

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1),
            retryCount: 5);

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(delay);
    }
}