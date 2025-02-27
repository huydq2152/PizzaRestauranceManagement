using System.Diagnostics;
using PlantBasedPizza.Order.Core.AddItemToOrder;

namespace PlantBasedPizza.Order.Infrastructure;

public static class ObservabilityExtensions
{
    public static void AddToTelemetry(this AddItemToOrderCommand command)
    {
        if (Activity.Current is null)
        {
            return;
        }

        Activity.Current.AddTag("orderIdentifier", command.OrderIdentifier);
        Activity.Current.AddTag("recipeIdentifier", command.RecipeIdentifier);
        Activity.Current.AddTag("quantity", command.Quantity);
    }
}