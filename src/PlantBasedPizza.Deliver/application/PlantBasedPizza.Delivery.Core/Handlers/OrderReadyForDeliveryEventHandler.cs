using PlantBasedPizza.Delivery.Core.Entities;
using PlantBasedPizza.Delivery.Core.IntegrationEvents;
using PlantBasedPizza.Shared.Logging;
using Saunter.Attributes;

namespace PlantBasedPizza.Delivery.Core.Handlers;

[AsyncApi]
public class OrderReadyForDeliveryEventHandler(
    IDeliveryRequestRepository deliveryRequestRepository,
    IObservabilityService logger)
{
    public async Task Handle(OrderReadyForDeliveryEventV1 evt)
    {
        if (evt == null)
        {
            throw new ArgumentNullException(nameof(evt), "Handled event cannot be null");
        }
            
        logger.Info($"Received new ready for delivery event for order {evt.OrderIdentifier}");

        var existingDeliveryRequestForOrder =
            await deliveryRequestRepository.GetDeliveryStatusForOrder(evt.OrderIdentifier);

        if (existingDeliveryRequestForOrder != null)
        {
            logger.Info("Delivery request for order received, skipping");
            return;
        }

        logger.Info("Creating and storing delivery request");

        var request = new DeliveryRequest(evt.OrderIdentifier,
            new Address(evt.DeliveryAddressLine1, evt.DeliveryAddressLine2, evt.DeliveryAddressLine3,
                evt.DeliveryAddressLine4, evt.DeliveryAddressLine5, evt.Postcode));

        await deliveryRequestRepository.AddNewDeliveryRequest(request);

        logger.Info("Delivery request added");
    }
}