using System.Text.Json.Serialization;
using PlantBasedPizza.Order.Core.Entities;

namespace PlantBasedPizza.Order.Infrastructure;

[JsonSerializable(typeof(Core.Entities.Order))]
[JsonSerializable(typeof(OrderHistory))]
[JsonSerializable(typeof(OrderItem))]
[JsonSerializable(typeof(DeliveryDetails))]
public partial class OrderManagerSerializationContext : JsonSerializerContext;