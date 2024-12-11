using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace PlantBasedPizza.Events;

public static class Setup
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var hostName = configuration["Messaging:HostName"];

        if (hostName is null)
        {
            throw new EventBusConnectionException("", "Host name is null");
        }
        
        services.AddSingleton(new RabbitMqConnection(hostName!));
        services.Configure<RabbitMqSettings>(configuration.GetSection("Messaging"));
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        services.AddSingleton<RabbitMqEventSubscriber>();

        return services;
    }
}