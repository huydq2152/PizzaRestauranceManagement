using Grpc.Core;
using PlantBasedPizza.Api.Internal;
using PlantBasedPizza.LoyaltyPoints.Shared.Core;

namespace PlantBasedPizza.LoyaltyPoints.Internal.Services;

public class LoyaltyService(AddLoyaltyPointsCommandHandler handler, ICustomerLoyaltyPointsRepository repository) : Loyalty.LoyaltyBase
{
    public override async Task<GetCustomerLoyaltyPointsReply> GetCustomerLoyaltyPoints(GetCustomerLoyaltyPointsRequest request, ServerCallContext context)
    {
        var loyaltyPoints = await repository.GetCurrentPointsFor(request.CustomerIdentifier);

        return new GetCustomerLoyaltyPointsReply()
        {
            CustomerIdentifier = request.CustomerIdentifier,
            TotalPoints = Convert.ToDouble(loyaltyPoints?.TotalPoints ?? 0)
        };
    }
    
    public override async Task<AddLoyaltyPointsReply> AddLoyaltyPoints(AddLoyaltyPointsRequest request, ServerCallContext context)
    {
        var command = new AddLoyaltyPointsCommand()
        {
            OrderIdentifier = request.OrderIdentifier,
            CustomerIdentifier = request.CustomerIdentifier,
            OrderValue = (decimal)request.OrderValue
        };

        var result = await handler.Handle(command);
        
        return new AddLoyaltyPointsReply()
        {
            CustomerIdentifier = request.CustomerIdentifier,
            TotalPoints = (double)result.TotalPoints
        };
    }
}