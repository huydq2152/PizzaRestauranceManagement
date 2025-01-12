using System.Diagnostics;
using PlantBasedPizza.Delivery.Core.Commands;
using PlantBasedPizza.Events;

namespace PlantBasedPizza.Delivery.Infrastructure;

public static class ObservabilityExtensions
{
    public static void AddToTelemetry(this AssignDriverRequest request)
    {
        if (Activity.Current is null)
        {
            return;
        }

        Activity.Current.SetTag("orderIdentifier", request.OrderIdentifier);
        Activity.Current.SetTag("driverName", request.DriverName);
    }
    
    public static void AddToTelemetry(this MarkOrderDeliveredRequest request)
    {
        if (Activity.Current is null)
            return;

        Activity.Current.SetTag("orderIdentifier", request.OrderIdentifier);
    }
}