namespace PlantBasedPizza.E2ETests.Drivers
{
    public static class TestConstants
    {
        public static readonly string DefaultTestUrl = Environment.GetEnvironmentVariable("TEST_URL") ?? "http://localhost:5051";
        
        public static readonly string LoyaltyTestUrl = Environment.GetEnvironmentVariable("TEST_URL") ?? "http://localhost:5050";
    }
}