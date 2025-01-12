using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PlantBasedPizza.Events;
using PlantBasedPizza.Order.Core.AddItemToOrder;
using PlantBasedPizza.Order.Core.CollectOrder;
using PlantBasedPizza.Order.Core.CreateDeliveryOrder;
using PlantBasedPizza.Order.Core.CreatePickupOrder;
using PlantBasedPizza.Orders.IntegrationTest.ViewModels;
using PlantBasedPizza.Orders.Worker.IntegrationEvents;
using Serilog.Extensions.Logging;

namespace PlantBasedPizza.Orders.IntegrationTest.Drivers;

public class OrdersTestDriver
{
    private readonly IEventPublisher _eventPublisher;
    private readonly HttpClient _userHttpClient;
    private readonly HttpClient _staffHttpClient;

    public OrdersTestDriver()
    {
        _eventPublisher = new RabbitMqEventPublisher(new OptionsWrapper<RabbitMqSettings>(new RabbitMqSettings
        {
            ExchangeName = "dev.plantbasedpizza",
            HostName = "localhost"
        }), new Logger<RabbitMqEventPublisher>(new SerilogLoggerFactory()), new RabbitMqConnection("localhost"));
    }

    public async Task SimulateLoyaltyPointsUpdatedEvent(string customerIdentifier, decimal totalPoints)
    {
        await _eventPublisher.Publish(new CustomerLoyaltyPointsUpdatedEvent
        {
            CustomerIdentifier = customerIdentifier,
            TotalLoyaltyPoints = totalPoints
        });

        // Delay to allow for message processing
        await Task.Delay(TimeSpan.FromSeconds(2));
    }

    public async Task AddNewDeliveryOrder(string orderIdentifier, string customerIdentifier)
    {
        await _userHttpClient.PostAsync(new Uri($"{TestConstants.DefaultTestUrl}/order/deliver"), new StringContent(
            JsonConvert.SerializeObject(new CreateDeliveryOrder
            {
                OrderIdentifier = orderIdentifier,
                CustomerIdentifier = customerIdentifier,
                AddressLine1 = "My test address",
                AddressLine2 = string.Empty,
                AddressLine3 = string.Empty,
                AddressLine4 = string.Empty,
                AddressLine5 = string.Empty,
                Postcode = "TYi9PO"
            }), Encoding.UTF8, "application/json")).ConfigureAwait(false);
    }

    public async Task AddNewOrder(string orderIdentifier, string customerIdentifier)
    {
        await Task.Delay(TimeSpan.FromSeconds(5));

        await _userHttpClient.PostAsync(new Uri($"{TestConstants.DefaultTestUrl}/order/pickup"), new StringContent(
            JsonConvert.SerializeObject(new CreatePickupOrderCommand
            {
                OrderIdentifier = orderIdentifier,
                CustomerIdentifier = customerIdentifier
            }), Encoding.UTF8, "application/json")).ConfigureAwait(false);
    }

    public async Task AddItemToOrder(string orderIdentifier, string recipeIdentifier, int quantity)
    {
        await Task.Delay(TimeSpan.FromSeconds(5));

        await CheckRecipeExists(recipeIdentifier).ConfigureAwait(false);

        await _userHttpClient.PostAsync(new Uri($"{TestConstants.DefaultTestUrl}/order/{orderIdentifier}/items"),
            new StringContent(
                JsonConvert.SerializeObject(new AddItemToOrderCommand
                {
                    OrderIdentifier = orderIdentifier,
                    RecipeIdentifier = recipeIdentifier,
                    Quantity = quantity
                }), Encoding.UTF8, "application/json")).ConfigureAwait(false);
    }

    public async Task SubmitOrder(string orderIdentifier)
    {
        await _userHttpClient.PostAsync(new Uri($"{TestConstants.DefaultTestUrl}/order/{orderIdentifier}/submit"),
            new StringContent(string.Empty, Encoding.UTF8, "application/json")).ConfigureAwait(false);
    }

    public async Task CollectOrder(string orderIdentifier)
    {
        // Delay to allow async processing to catch up
        await Task.Delay(TimeSpan.FromSeconds(2));

        var res = await _staffHttpClient.PostAsync(new Uri($"{TestConstants.DefaultTestUrl}/order/collected"),
            new StringContent(
                JsonConvert.SerializeObject(new CollectOrderRequest
                {
                    OrderIdentifier = orderIdentifier
                }), Encoding.UTF8, "application/json")).ConfigureAwait(false);

        if (!res.IsSuccessStatusCode)
        {
            throw new Exception($"Collect order returned non 200 HTTP Status code: {res.StatusCode}");
        }
    }

    public async Task<ViewModels.Order> GetOrder(string orderIdentifier)
    {
        var result = await _userHttpClient
            .GetAsync(new Uri($"{TestConstants.DefaultTestUrl}/order/{orderIdentifier}/detail"))
            .ConfigureAwait(false);

        var order = JsonConvert.DeserializeObject<ViewModels.Order>(await result.Content.ReadAsStringAsync());

        return order;
    }

    private async Task CheckRecipeExists(string recipeIdentifier)
    {
        await _userHttpClient.PostAsync($"{TestConstants.DefaultTestUrl}/recipes", new StringContent(
            JsonConvert.SerializeObject(new CreateRecipeCommand
            {
                RecipeIdentifier = recipeIdentifier,
                Name = recipeIdentifier,
                Price = 10,
                Ingredients = new List<CreateRecipeCommandItem>(1)
                {
                    new()
                    {
                        Name = "Pizza",
                        Quantity = 1
                    }
                }
            }), Encoding.UTF8, "application/json"));
    }
}