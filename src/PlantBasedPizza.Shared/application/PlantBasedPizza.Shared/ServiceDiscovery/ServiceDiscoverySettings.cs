namespace PlantBasedPizza.Shared.ServiceDiscovery;

public record ServiceDiscoverySettings
{
    private string _serviceId;
    
    public string MyUrl { get; set; }
    public string ConsulServiceEndpoint { get; set; }
    public string ServiceName { get; set; }

    public string ServiceId
    {
        get
        {
            if (string.IsNullOrEmpty(_serviceId))
            {
                _serviceId = $"{ServiceName}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            }

            return _serviceId;
        }
    }
}