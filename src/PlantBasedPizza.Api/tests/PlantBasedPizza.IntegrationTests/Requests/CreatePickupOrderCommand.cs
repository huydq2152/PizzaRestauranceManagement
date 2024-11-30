namespace PlantBasedPizza.IntegrationTests.Requests
{
    public class CreatePickupOrderCommand
    {
        public string OrderIdentifier { get; set; }
        
        public string CustomerIdentifier { get; set; }

        public int OrderType = 0;
    }
}