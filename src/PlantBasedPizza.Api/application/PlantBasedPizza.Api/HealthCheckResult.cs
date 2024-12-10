using PlantBasedPizza.Order.Infrastructure;

namespace PlantBasedPizza.Api;

/// <summary>
/// Represents the result of a health check.
/// </summary>
public record HealthCheckResult
{
    /// <summary>
    /// Gets or sets the order manager health check result.
    /// </summary>
    public OrderManagerHealthCheckResult? OrderManagerHealthCheck { get; set; }
}