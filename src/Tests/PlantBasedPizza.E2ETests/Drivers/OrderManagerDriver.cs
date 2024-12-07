using System.Text;
using Newtonsoft.Json;
using PlantBasedPizza.E2ETests.Requests;
using PlantBasedPizza.E2ETests.ViewModels;

namespace PlantBasedPizza.E2ETests.Drivers
{
    public class OrderManagerDriver
    {
        private static readonly string BaseUrl = TestConstants.DefaultTestUrl;

        private readonly HttpClient _httpClient = new();

        public async Task AddNewDeliveryOrder(string orderIdentifier, string customerIdentifier)
        {
            await _httpClient.PostAsync(new Uri($"{BaseUrl}/order/deliver"), new StringContent(
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
            await _httpClient.PostAsync(new Uri($"{BaseUrl}/order/pickup"), new StringContent(
                JsonConvert.SerializeObject(new CreatePickupOrderCommand
                {
                    OrderIdentifier = orderIdentifier,
                    CustomerIdentifier = customerIdentifier
                }), Encoding.UTF8, "application/json")).ConfigureAwait(false);
        }

        public async Task AddItemToOrder(string orderIdentifier, string recipeIdentifier, int quantity)
        {
            await CheckRecipeExists(recipeIdentifier).ConfigureAwait(false);

            await _httpClient.PostAsync(new Uri($"{BaseUrl}/order/{orderIdentifier}/items"),
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
            await _httpClient.PostAsync(new Uri($"{BaseUrl}/order/{orderIdentifier}/submit"),
                new StringContent(string.Empty, Encoding.UTF8, "application/json")).ConfigureAwait(false);
        }

        public async Task CollectOrder(string orderIdentifier)
        {
            var res = await _httpClient.PostAsync(new Uri($"{BaseUrl}/order/collected"), new StringContent(
                JsonConvert.SerializeObject(new CollectOrderRequest
                {
                    OrderIdentifier = orderIdentifier
                }), Encoding.UTF8, "application/json")).ConfigureAwait(false);

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception($"Collect order returned non 200 HTTP Status code: {res.StatusCode}");
            }
        }

        public async Task<Order> GetOrder(string orderIdentifier)
        {
            var result = await _httpClient.GetAsync(new Uri($"{BaseUrl}/order/{orderIdentifier}/detail"))
                .ConfigureAwait(false);

            var order = JsonConvert.DeserializeObject<Order>(await result.Content.ReadAsStringAsync());

            return order;
        }

        private async Task CheckRecipeExists(string recipeIdentifier)
        {
            await _httpClient.PostAsync($"{BaseUrl}/recipes", new StringContent(
                JsonConvert.SerializeObject(new CreateRecipeCommand
                {
                    RecipeIdentifier = recipeIdentifier,
                    Name = recipeIdentifier,
                    Price = 10,
                    Ingredients = new List<CreateRecipeCommandItem>(1)
                    {
                        new CreateRecipeCommandItem
                        {
                            Name = "Pizza",
                            Quantity = 1
                        }
                    }
                }), Encoding.UTF8, "application/json"));
        }
    }
}