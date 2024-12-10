using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PlantBasedPizza.Shared.ServiceDiscovery;

public class ConsulRegisterService(
    IConsulClient consulClient,
    IOptions<ServiceDiscoverySettings> discoverySettings,
    ILogger<ConsulRegisterService> logger)
    : IHostedService
{
    private readonly ServiceDiscoverySettings _discoverySettings = discoverySettings.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var myUri = new Uri(_discoverySettings.MyUrl);

        var serviceRegistration = new AgentServiceRegistration()
        {
            Address = myUri.Host,
            Name = _discoverySettings.ServiceName,
            Port = myUri.Port,
            ID = _discoverySettings.ServiceId,
            Tags = new[] { _discoverySettings.ServiceName }
        };
        
        await consulClient.Agent.ServiceDeregister(_discoverySettings.ServiceId, cancellationToken);
        await consulClient.Agent.ServiceRegister(serviceRegistration, cancellationToken);
        
        logger.LogInformation("Service Registered with Consul");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await consulClient.Agent.ServiceDeregister(_discoverySettings.ServiceId, cancellationToken);
            
            logger.LogInformation("Service de-registered from Consul");
        }
        catch(Exception ex)
        {
            logger.LogError("Failure de-registering service", ex);
        }
    }
}