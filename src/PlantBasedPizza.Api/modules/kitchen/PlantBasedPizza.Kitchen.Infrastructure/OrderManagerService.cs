using PlantBasedPizza.Kitchen.Core.Adapters;
using PlantBasedPizza.Kitchen.Core.Services;
using PlantBasedPizza.OrderManager.Core.Entities;

namespace PlantBasedPizza.Kitchen.Infrastructure;

public class OrderManagerService(IOrderRepository orderRepo) : IOrderManagerService
{
    public async Task<OrderAdapter> GetOrderDetails(string orderIdentifier)
    {
        var order = await orderRepo.Retrieve(orderIdentifier).ConfigureAwait(false);

        var orderAdapter = new OrderAdapter();

        foreach (var orderItem in order.Items)
        {
            orderAdapter.Items.Add(new OrderItemAdapter()
            {
                ItemName = orderItem.ItemName,
                RecipeIdentifier = orderItem.RecipeIdentifier
            });
        }

        return orderAdapter;
    }
}