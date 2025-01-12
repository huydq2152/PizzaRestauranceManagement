using Microsoft.AspNetCore.Builder;
using PlantBasedPizza.Events;
using PlantBasedPizza.Order.Infrastructure;
using PlantBasedPizza.Orders.Worker;
using PlantBasedPizza.Orders.Worker.Handlers;
using PlantBasedPizza.Shared;
using DriverCollectedOrderEventHandler = PlantBasedPizza.Orders.Worker.Handlers.DriverCollectedOrderEventHandler;
using DriverDeliveredOrderEventHandler = PlantBasedPizza.Orders.Worker.Handlers.DriverDeliveredOrderEventHandler;
using OrderBakedEventHandler = PlantBasedPizza.Orders.Worker.Handlers.OrderBakedEventHandler;
using OrderPrepCompleteEventHandler = PlantBasedPizza.Orders.Worker.Handlers.OrderPrepCompleteEventHandler;
using OrderQualityCheckedEventHandler = PlantBasedPizza.Orders.Worker.Handlers.OrderQualityCheckedEventHandler;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

var serviceName = "OrdersWorker";

builder.Services
    .AddSharedInfrastructure(builder.Configuration, serviceName)
    .AddMessaging(builder.Configuration)
    .AddOrderManagerInfrastructure(builder.Configuration);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisConnectionString"];
    options.InstanceName = "Orders";
});

builder.Services.AddSingleton<DriverCollectedOrderEventHandler>();
builder.Services.AddSingleton<DriverDeliveredOrderEventHandler>();
builder.Services.AddSingleton<OrderBakedEventHandler>();
builder.Services.AddSingleton<OrderPreparingEventHandler>();
builder.Services.AddSingleton<OrderPrepCompleteEventHandler>();
builder.Services.AddSingleton<OrderQualityCheckedEventHandler>();

builder.Services.AddHostedService<LoyaltyPointsUpdatedCacheWorker>();
builder.Services.AddHostedService<DriverCollectedOrderEventWorker>();
builder.Services.AddHostedService<DriverDeliveredOrderEventWorker>();
builder.Services.AddHostedService<OrderBakedEventWorker>();
builder.Services.AddHostedService<OrderPreparingEventWorker>();
builder.Services.AddHostedService<OrderPrepCompleteEventWorker>();
builder.Services.AddHostedService<OrderQualityCheckedEventWorker>();

var app = builder.Build();

app.MapGet("/orders/health", () => "Healthy");

app.Run();