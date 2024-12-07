using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

using PlantBasedPizza.Deliver.Infrastructure;
using PlantBasedPizza.Kitchen.Infrastructure;
using PlantBasedPizza.OrderManager.Infrastructure;
using PlantBasedPizza.Recipes.Infrastructure;
using PlantBasedPizza.Shared;
using PlantBasedPizza.Shared.Events;
using PlantBasedPizza.Shared.Logging;
using HealthCheckResult = PlantBasedPizza.Api.HealthCheckResult;

var builder = WebApplication.CreateBuilder(args);
builder
    .Configuration
    .AddEnvironmentVariables();

var client = new MongoClient(builder.Configuration["DatabaseConnection"]);

builder.Services.AddSingleton(client);

builder.Services.AddOrderManagerInfrastructure(builder.Configuration);
builder.Services.AddRecipeInfrastructure(builder.Configuration);
builder.Services.AddKitchenInfrastructure(builder.Configuration);
builder.Services.AddDeliveryModuleInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration, "PlantBasedPizza");
builder.Services.AddHttpClient();

builder.Services.AddControllers();

var app = builder.Build();

DomainEvents.Container = app.Services;

var orderManagerHealthChecks = app.Services.GetRequiredService<OrderManagerHealthChecks>();

app.Map("/health", async () =>
{
    var healthCheckResult = new HealthCheckResult
    {
        OrderManagerHealthCheck = await orderManagerHealthChecks.Check()
    };

    return Results.Ok(healthCheckResult);
});

app.Use(async (context, next) =>
{
    var observability = app.Services.GetService<IObservabilityService>();

    var correlationId = string.Empty;

    if (context.Request.Headers.ContainsKey(CorrelationContext.DefaultRequestHeaderName))
    {
        correlationId = context.Request.Headers[CorrelationContext.DefaultRequestHeaderName].ToString();
    }
    else
    {
        correlationId = Guid.NewGuid().ToString();

        context.Request.Headers.Append(CorrelationContext.DefaultRequestHeaderName, correlationId);
    }

    CorrelationContext.SetCorrelationId(correlationId);

    observability?.Info($"Request received to {context.Request.Path.Value}");

    context.Response.Headers.Append(CorrelationContext.DefaultRequestHeaderName, correlationId);

    // Do work that doesn't write to the Response.
    await next.Invoke();
});

app.MapControllers();

app.Run();