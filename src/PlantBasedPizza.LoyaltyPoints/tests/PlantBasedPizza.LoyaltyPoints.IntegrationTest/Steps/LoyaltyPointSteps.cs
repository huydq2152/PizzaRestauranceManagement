using FluentAssertions;
using PlantBasedPizza.LoyaltyPoints.IntegrationTest.Drivers;
using TechTalk.SpecFlow;

namespace PlantBasedPizza.LoyaltyPoints.IntegrationTest.Steps;

[Binding]
public sealed class LoyaltyPointSteps
{
    private readonly LoyaltyPointsDriver _driver;

    public LoyaltyPointSteps()
    {
        _driver = new LoyaltyPointsDriver();
    }
    
    [Given(@"the loyalty points are added for customer (.*) for order (.*) with a value of (.*)")]
    public async Task LoyaltyPointsAreAdded(string customerId, string orderIdentifier, decimal orderValue)
    {
        await _driver.AddLoyaltyPoints(customerId, orderIdentifier, orderValue);
    }

    [Then(@"the total points should be (.*) for (.*)")]
    public async Task ThenTheTotalPointsShouldBe(int totalPoints, string customerIdentifier)
    {
        var points = await _driver.GetLoyaltyPoints(customerIdentifier);

        points.TotalPoints.Should().Be(totalPoints);
    }

    [When(@"(.*) points are spent for customer (.*) for order (.*)")]
    public async Task WhenPointsAreSpentForCustomerJamesForOrderOrd(int points, string customerId, string orderIdentifier)
    {
        await _driver.SpendLoyaltyPoints(customerId, orderIdentifier, points);
    }
}