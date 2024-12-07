using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlantBasedPizza.OrderManager.Core.AddItemToOrder;
using PlantBasedPizza.OrderManager.Core.CollectOrder;
using PlantBasedPizza.OrderManager.Core.CreateDeliveryOrder;
using PlantBasedPizza.OrderManager.Core.CreatePickupOrder;
using PlantBasedPizza.OrderManager.Core.Entities;

namespace PlantBasedPizza.OrderManager.Infrastructure.Controllers;

[Route("order")]
public class OrderController(
    IOrderRepository orderRepository,
    CollectOrderCommandHandler collectOrderCommandHandler,
    AddItemToOrderHandler addItemToOrderHandler,
    CreateDeliveryOrderCommandHandler createDeliveryOrderCommandHandler,
    CreatePickupOrderCommandHandler createPickupOrderCommandHandler)
    : ControllerBase
{
    /// <summary>
    /// Get the details of a given order.
    /// </summary>
    /// <param name="orderIdentifier">The order identifier.</param>
    /// <returns></returns>
    [HttpGet("{orderIdentifier}/detail")]
    public async Task<OrderDto?> Get(string orderIdentifier)
    {
        try
        {
            Activity.Current?.SetTag("orderIdentifier", orderIdentifier);
                
            var order = await orderRepository.Retrieve(orderIdentifier).ConfigureAwait(false);
                
            return new OrderDto(order);
        }
        catch (OrderNotFoundException)
        {
            Response.StatusCode = 404;
            Activity.Current?.AddTag("order.notFound", true);

            return null;
        }
    }

    /// <summary>
    /// Create a new order for pickup.
    /// </summary>
    /// <param name="request">The <see cref="CreatePickupOrderCommand"/> command contents.</param>
    /// <returns></returns>
    [HttpPost("pickup")]
    public async Task<OrderDto?> Create([FromBody] CreatePickupOrderCommand request) =>
        await createPickupOrderCommandHandler.Handle(request);

    /// <summary>
    /// Create a new delivery order.
    /// </summary>
    /// <param name="request">The <see cref="CreateDeliveryOrder"/> request.</param>
    /// <returns></returns>
    [HttpPost("deliver")]
    public async Task<OrderDto?> Create([FromBody] CreateDeliveryOrder request) =>
        await createDeliveryOrderCommandHandler.Handle(request);

    /// <summary>
    /// Add an item to the order.
    /// </summary>
    /// <param name="request">the <see cref="AddItemToOrderCommand"/> request.</param>
    /// <returns></returns>
    [HttpPost("{orderIdentifier}/items")]
    public async Task<OrderDto?> AddItemToOrder([FromBody] AddItemToOrderCommand request)
    {
        request.AddToTelemetry();

        var order = await addItemToOrderHandler.Handle(request);

        if (order is null)
        {
            Response.StatusCode = 404;
        }

        return new OrderDto(order);
    }
        
    /// <summary>
    /// Submit an order.
    /// </summary>
    /// <param name="orderIdentifier">The order to submit.</param>
    /// <returns></returns>
    [HttpPost("{orderIdentifier}/submit")]
    public async Task<OrderDto> SubmitOrder(string orderIdentifier)
    {
        var order = await orderRepository.Retrieve(orderIdentifier);

        order.SubmitOrder();

        await orderRepository.Update(order);

        return new OrderDto(order);
    }

    /// <summary>
    /// List all orders awaiting collection.
    /// </summary>
    /// <returns></returns>
    [HttpGet("awaiting-collection")]
    public async Task<IEnumerable<OrderDto>> GetAwaitingCollection()
    {
        var awaitingCollection = await orderRepository.GetAwaitingCollection();

        return awaitingCollection.Select(order => new OrderDto(order));
    }

    /// <summary>
    /// Mark an order as being collected.
    /// </summary>
    /// <param name="request">The <see cref="CollectOrderRequest"/> request.</param>
    /// <returns></returns>
    [HttpPost("collected")]
    public async Task<OrderDto?> OrderCollected([FromBody] CollectOrderRequest request) =>
        await collectOrderCommandHandler.Handle(request);
}