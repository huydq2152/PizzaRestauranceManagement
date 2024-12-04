namespace PlantBasedPizza.LoyaltyPoints.Shared.Core;

public class SpendLoyaltyPointsCommandHandler(ICustomerLoyaltyPointsRepository customerLoyaltyPointsRepository)
{
    public async Task<LoyaltyPointsDto> Handle(SpendLoyaltyPointsCommand command)
    {
        var currentLoyaltyPoints = await customerLoyaltyPointsRepository.GetCurrentPointsFor(command.CustomerIdentifier) ??
                                   CustomerLoyaltyPoints.Create(command.CustomerIdentifier);

        currentLoyaltyPoints.SpendPoints(command.PointsToSpend, command.OrderIdentifier);

        await customerLoyaltyPointsRepository.UpdatePoints(currentLoyaltyPoints);

        return new LoyaltyPointsDto(currentLoyaltyPoints);
    }
}