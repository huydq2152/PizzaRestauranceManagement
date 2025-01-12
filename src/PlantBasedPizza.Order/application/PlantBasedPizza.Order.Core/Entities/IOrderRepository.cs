namespace PlantBasedPizza.Order.Core.Entities;

public interface IOrderRepository
{
    Task Add(Order order);

    Task<Order> Retrieve(string orderIdentifier);
    
    Task<Order> Exists(string orderIdentifier);

    Task<List<Order>> GetAwaitingCollection();
        
    Task Update(Order order);
}