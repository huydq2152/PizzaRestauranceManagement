namespace PlantBasedPizza.E2ETests.Drivers;

public class HealthCheckDriver
{
    private static readonly string BaseUrl = TestConstants.DefaultTestUrl;

    private readonly HttpClient _httpClient = new();

    public async Task<int> HealthCheck(bool loyalyPointSuccess = true)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{BaseUrl}/health"));
        httpRequestMessage.Headers.Add("Response", loyalyPointSuccess ? "Success" : "Failure");
            
        var result = await _httpClient
            .SendAsync(httpRequestMessage)
            .ConfigureAwait(false);

        return (int)result.StatusCode;
    }
}