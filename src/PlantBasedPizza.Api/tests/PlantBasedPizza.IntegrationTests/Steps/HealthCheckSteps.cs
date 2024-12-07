using System.Threading.Tasks;
using FluentAssertions;
using PlantBasedPizza.IntegrationTests.Drivers;
using TechTalk.SpecFlow;

namespace PlantBasedPizza.IntegrationTests.Steps;

[Binding]
public class HealthCheckSteps
{
    private readonly HealthCheckDriver _driver;
    private bool _loyaltyPointOnline = true;

    public HealthCheckSteps(ScenarioContext scenarioContext)
    {
        _driver = new HealthCheckDriver();
    }
    
    [Given(@"the application is running")]
    public void GivenTheApplicationIsRunning()
    {
        // Given required to startup the application.
    }
    
    [When(@"the loyalty point service is offline")]
    public void WhenTheLoyaltyPointServiceIsOffline()
    {
        _loyaltyPointOnline = false;
    }

    [Then(@"a (.*) status code is returned")]
    public async Task ThenAStatusCodeIsReturned(int p0)
    {
        var res = await _driver.HealthCheck(_loyaltyPointOnline);

        res.Should().Be(p0);
    }
}