namespace PlantBasedPizza.LoyaltyPoints.Shared.Core;

public class AddLoyaltyPointsCommandHandler(ICustomerLoyaltyPointsRepository customerLoyaltyPointsRepository)
{
    public async Task<LoyaltyPointsDto> Handle(AddLoyaltyPointsCommand command)
    {
        var currentLoyaltyPoints = await customerLoyaltyPointsRepository.GetCurrentPointsFor(command.CustomerIdentifier) ??
                                   CustomerLoyaltyPoints.Create(command.CustomerIdentifier);

        currentLoyaltyPoints.AddLoyaltyPoints(command.OrderValue, command.OrderIdentifier);

        await customerLoyaltyPointsRepository.UpdatePoints(currentLoyaltyPoints);

        return new LoyaltyPointsDto(currentLoyaltyPoints);
    }
}