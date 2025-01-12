using System.Diagnostics;
using FluentAssertions;
using PlantBasedPizza.LoyaltyPoints.IntegrationTest.Drivers;
using TechTalk.SpecFlow;

namespace PlantBasedPizza.LoyaltyPoints.IntegrationTest.Steps;

[Binding]
public sealed class LoyaltyPointSteps(ScenarioContext scenarioContext)
{
    private readonly LoyaltyPointsDriver _driver = new();

    [Given(@"the loyalty points are added for customer (.*) for order (.*) with a value of (.*)")]
    public async Task LoyaltyPointsAreAdded(string customerId, string orderIdentifier, decimal orderValue)
    {
        Activity.Current = scenarioContext.Get<Activity>("Activity");
        await _driver.AddLoyaltyPoints(customerId, orderValue);
    }

    [Then(@"the total points should be (.*) for (.*)")]
    public async Task ThenTheTotalPointsShouldBe(int totalPoints, string customerIdentifier)
    {
        Activity.Current = scenarioContext.Get<Activity>("Activity");
        
        var points = await _driver.GetLoyaltyPoints();
        var internalPoints = await _driver.GetLoyaltyPointsInternal();

        points.TotalPoints.Should().Be(totalPoints);
    }

    [When(@"(.*) points are spent for customer (.*) for order (.*)")]
    public async Task WhenPointsAreSpentForCustomerJamesForOrderOrd(int points, string customerId, string orderIdentifier)
    {
        Activity.Current = scenarioContext.Get<Activity>("Activity");
        await _driver.SpendLoyaltyPoints(customerId, orderIdentifier, points);
    }
}