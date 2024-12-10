using PlantBasedPizza.Kitchen.Core.Entities;

namespace PlantBasedPizza.Kitchen.Infrastructure.DataTransfer;

public class KitchenRequestDto
{
    public KitchenRequestDto()
    {
        KitchenRequestId = "";
        OrderIdentifier = "";
    }

    public KitchenRequestDto(KitchenRequest request)
    {
        KitchenRequestId = request.KitchenRequestId;
        OrderIdentifier = request.OrderIdentifier;
        OrderReceivedOn = request.OrderReceivedOn;
        PrepCompleteOn = request.PrepCompleteOn;
        BakeCompleteOn = request.BakeCompleteOn;
        QualityCheckCompleteOn = request.QualityCheckCompleteOn;
    }

    public string KitchenRequestId { get; set; }
    
    public string OrderIdentifier { get; set; }
    
    public DateTime OrderReceivedOn { get; set; }
        
    public DateTime? PrepCompleteOn { get; set; }
        
    public DateTime? BakeCompleteOn { get; set; }
        
    public DateTime? QualityCheckCompleteOn { get; set; }
}