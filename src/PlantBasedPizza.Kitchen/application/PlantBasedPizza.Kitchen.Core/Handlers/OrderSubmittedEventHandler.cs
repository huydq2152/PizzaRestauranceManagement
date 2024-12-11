using PlantBasedPizza.Events;
using PlantBasedPizza.Events.IntegrationEvents;
using PlantBasedPizza.Kitchen.Core.Adapters;
using PlantBasedPizza.Kitchen.Core.Entities;
using PlantBasedPizza.Kitchen.Core.Services;
using PlantBasedPizza.Shared.Guards;
using PlantBasedPizza.Shared.Logging;
using Saunter.Attributes;

namespace PlantBasedPizza.Kitchen.Core.Handlers;

[AsyncApi]
public class OrderSubmittedEventHandler(
    IKitchenRequestRepository kitchenRequestRepository,
    IRecipeService recipeService,
    IObservabilityService logger,
    IOrderManagerService orderManagerService)
    : IHandles<OrderSubmittedEvent>
{
    [Channel("order-manager.order-submitted")] // Creates a Channel
    [SubscribeOperation(typeof(OrderSubmittedEvent), Summary = "Handle an order submitted event.", OperationId = "order-manager.order-submitted")]
    public async Task Handle(OrderSubmittedEvent evt)
    {
        Guard.AgainstNull(evt, nameof(evt));

        logger.Info("[KITCHEN] Logging order submitted event");

        var recipes = new List<RecipeAdapter>();
            
        var order = await orderManagerService.GetOrderDetails(evt.OrderIdentifier);
            
        logger.Info($"[KITCHEN] Order has {order.Items.Count} item(s)");

        foreach (var recipe in order.Items)
        {
            logger.Info($"[KITCHEN] Addig item {recipe.ItemName}");
                
            recipes.Add(await recipeService.GetRecipe(recipe.RecipeIdentifier));
        }

        var kitchenRequest = new KitchenRequest(evt.OrderIdentifier, recipes);

        logger.Info("[KITCHEN] Storing kitchen request");

        await kitchenRequestRepository.AddNew(kitchenRequest);
    }
}