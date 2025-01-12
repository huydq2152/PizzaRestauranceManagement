using System.Text;
using System.Text.Json;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlantBasedPizza.Events;
using PlantBasedPizza.LoyaltyPoints.IntegrationTest.LoyaltyClient;
using PlantBasedPizza.LoyaltyPoints.IntegrationTest.ViewModels;
using Serilog.Extensions.Logging;

namespace PlantBasedPizza.LoyaltyPoints.IntegrationTest.Drivers;

public class LoyaltyPointsDriver
    {
        private static readonly string BaseUrl = TestConstants.DefaultTestUrl;

        private readonly HttpClient _httpClient = new();
        private readonly Loyalty.LoyaltyClient _loyaltyClient = new(GrpcChannel.ForAddress(TestConstants.InternalTestEndpoint));
        private readonly IEventPublisher _eventPublisher = new RabbitMqEventPublisher(new OptionsWrapper<RabbitMqSettings>(new RabbitMqSettings
        {
            ExchangeName = "dev.loyalty",
            HostName = "localhost"
        }), new Logger<RabbitMqEventPublisher>(new SerilogLoggerFactory()), new RabbitMqConnection("localhost"));

        public async Task AddLoyaltyPoints(string orderIdentifier, decimal orderValue)
        {
            await _eventPublisher.Publish(new OrderCompletedIntegrationEventV1
            {
                CustomerIdentifier = "user-account",
                OrderIdentifier = orderIdentifier,
                OrderValue = orderValue
            });
        }

        public async Task<LoyaltyPointsDto?> GetLoyaltyPointsInternal()
        {
            // Delay to allow for message processing
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            var points = await _loyaltyClient.GetCustomerLoyaltyPointsAsync(new GetCustomerLoyaltyPointsRequest
            {
                CustomerIdentifier = "user-account"
            });

            return new LoyaltyPointsDto
            {
                CustomerIdentifier = points.CustomerIdentifier,
                TotalPoints = Convert.ToDecimal(points.TotalPoints)
            };
        }

        public async Task<LoyaltyPointsDto?> GetLoyaltyPoints()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            var url = $"{BaseUrl}/loyalty";
            
            var getResult = await _httpClient.GetAsync(new Uri(url)).ConfigureAwait(false);

            return JsonSerializer.Deserialize<LoyaltyPointsDto>(await getResult.Content.ReadAsStringAsync());
        }

        public async Task SpendLoyaltyPoints(string customerIdentifier, string orderIdentifier, int points)
        {
            var url = $"{BaseUrl}/loyalty/spend";
            
            var content = JsonSerializer.Serialize(new SpendLoyaltyPointsCommand {CustomerIdentifier = customerIdentifier, OrderIdentifier = orderIdentifier, PointsToSpend = points});
            
            await _httpClient.PostAsync(new Uri(url), new StringContent(content, Encoding.UTF8, "application/json")).ConfigureAwait(false);
        }
    }