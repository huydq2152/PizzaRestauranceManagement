using System.Security.Cryptography;
using Consul;
using Microsoft.Extensions.Logging;

namespace PlantBasedPizza.Shared.ServiceDiscovery;

public class ConsulServiceRegistry(IConsulClient consulClient, ILogger<ConsulServiceRegistry> logger)
    : IServiceRegistry
{
    public async Task<string?> GetServiceAddress(string serviceName)
    {
        var services = await consulClient.Health.Service(serviceName);
        
        logger.LogInformation($"Found {services.Response.Length} service(s) for {serviceName}");

        if (services.Response.Length == 0)
        {
            return null;
        }
        
        if (services.Response.Length == 1)
        {
            var singleService = services.Response[0];
            
            logger.LogInformation($"Returning address: {singleService.Service.Address}:{singleService.Service.Port}");
            
            return $"http://{singleService.Service.Address}:{singleService.Service.Port}";
        }

        var indexToUse = RandomNumberGenerator.GetInt32(0, services.Response.Length - 1);

        var service = services.Response[indexToUse];
        
        logger.LogInformation($"Returning address: {service.Service.Address}");
        
        return $"http://{service.Service.Address}:{service.Service.Port}";
    }
}