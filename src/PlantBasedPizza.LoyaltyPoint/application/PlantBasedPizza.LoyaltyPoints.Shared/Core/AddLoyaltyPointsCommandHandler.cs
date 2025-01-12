using Microsoft.Extensions.Logging;

namespace PlantBasedPizza.LoyaltyPoints.Shared.Core;

public class AddLoyaltyPointsCommandHandler(ICustomerLoyaltyPointsRepository customerLoyaltyPointsRepository, ILogger<AddLoyaltyPointsCommandHandler> logger)
{
    public async Task<LoyaltyPointsDto> Handle(AddLoyaltyPointsCommand command)
    {
        logger.LogInformation($"Handling AddLoyaltyPointsCommand for {command.OrderIdentifier}");
        var currentLoyaltyPoints = await customerLoyaltyPointsRepository.GetCurrentPointsFor(command.CustomerIdentifier) ??
                                   CustomerLoyaltyPoints.Create(command.CustomerIdentifier);

        currentLoyaltyPoints.AddLoyaltyPoints(command.OrderValue, command.OrderIdentifier);

        await customerLoyaltyPointsRepository.UpdatePoints(currentLoyaltyPoints);

        return new LoyaltyPointsDto(currentLoyaltyPoints);
    }
}