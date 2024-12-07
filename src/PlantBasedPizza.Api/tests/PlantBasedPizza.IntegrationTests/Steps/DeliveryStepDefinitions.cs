using System;
using System.Threading.Tasks;
using FluentAssertions;
using PlantBasedPizza.IntegrationTests.Drivers;
using TechTalk.SpecFlow;

namespace PlantBasedPizza.IntegrationTests.Steps;

[Binding]
public sealed class DeliveryStepDefinitions
{
    private readonly DeliveryDriver _driver;

    public DeliveryStepDefinitions(ScenarioContext scenarioContext)
    {
        _driver = new DeliveryDriver();
    }

    [Then(@"order (.*) should be awaiting delivery collection")]
    public async Task ThenOrderDeliverShouldBeAwaitingDeliveryCollection(string p0)
    {
        var ordersAwaitingDriver = await _driver.GetAwaitingDriver();

        ordersAwaitingDriver.Exists(p => p.OrderIdentifier == p0).Should().BeTrue();
    }

    [When(@"order (.*) is assigned to a driver named (.*)")]
    public async Task WhenOrderDeliverIsAssignedToADriverNamedJames(string p0, string p1)
    {
        await _driver.AssignDriver(p0, p1);
    }

    [Then(@"order (.*) should appear in a list of (.*) deliveries")]
    public async Task ThenOrderDeliverShouldAppearInAListOfJamesDeliveries(string p0, string p1)
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
            
        var ordersForDriver = await _driver.GetAssignedDeliveriesForDriver(p1);

        ordersForDriver.Exists(p => p.OrderIdentifier == p0).Should().BeTrue();
    }

    [When(@"order (.*) is delivered")]
    public async Task WhenOrderDeliverIsDelivered(string p0)
    {
        await _driver.DeliverOrder(p0);
    }

    [Then(@"order (.*) should no longer be assigned to a driver named (.*)")]
    public async Task ThenOrderDeliverShouldNoLongerBeAssignedToADriverNamedJames(string p0, string p1)
    {
        var ordersForDriver = await _driver.GetAssignedDeliveriesForDriver(p1);

        ordersForDriver.Exists(p => p.OrderIdentifier == p0).Should().BeFalse();
    }
}