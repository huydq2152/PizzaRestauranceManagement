using FluentAssertions;
using PlantBasedPizza.Payments.IntegrationTests.Drivers;
using TechTalk.SpecFlow;

namespace PlantBasedPizza.Payments.IntegrationTests.Steps;

[Binding]
public sealed class PaymentSteps
{
    private readonly PaymentDriver _driver = new();

    [Then(@"a payment is taken for (.*) then the result should be successful")]
    public async Task ThenAPaymentIsTakenForThenTheResultShouldBeSuccessful(double p0)
    {
        var result = await _driver.TakePaymentFor("James", p0);
        
        result.Should().BeTrue();
    }
}