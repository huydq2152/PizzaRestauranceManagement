using PlantBasedPizza.Order.Core.Entities;
using PlantBasedPizza.Order.Core.Services;

namespace PlantBasedPizza.Order.Core.AddItemToOrder;

public class AddItemToOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRecipeService _recipeService;

    public AddItemToOrderHandler(IOrderRepository orderRepository, IRecipeService recipeService)
    {
        _orderRepository = orderRepository;
        _recipeService = recipeService;
    }
    
    public async Task<Entities.Order?> Handle(AddItemToOrderCommand command)
    {
        try
        {
            var recipe = await this._recipeService.GetRecipe(command.RecipeIdentifier);
            
            var order = await this._orderRepository.Retrieve(command.OrderIdentifier);

            order.AddOrderItem(command.RecipeIdentifier, recipe.ItemName, command.Quantity, recipe.Price);

            await this._orderRepository.Update(order);

            return order;
        }
        catch (OrderNotFoundException)
        {
            return null;
        }
    }
}