using FluentAssertions;
using Moq;
using PlantBasedPizza.Delivery.Core.Entities;
using PlantBasedPizza.Delivery.Core.Handlers;
using PlantBasedPizza.Delivery.Core.IntegrationEvents;
using PlantBasedPizza.Shared.Logging;

namespace PlantBasedPizza.Delivery.UnitTests
{
    public class DeliveryRequestTests
    {
        internal const string OrderIdentifier = "ORDER123";
        
        [Fact]
        public void CanCreateNewDeliveryRequest_ShouldCreate()
        {
            var request = new DeliveryRequest(OrderIdentifier, new Address("Address line 1", "TY6 7UI"));

            request.OrderIdentifier.Should().Be(OrderIdentifier);
            request.DeliveryAddress.Should().NotBeNull();
            request.DeliveryAddress.AddressLine1.Should().Be("Address line 1");
            request.DeliveryAddress.Postcode.Should().Be("TY6 7UI");
        }
        
        [Fact]
        public void CanCreateNewDeliveryRequestAddAddDriver_ShouldAddDriverAndRaiseEvent()
        {
            var request = new DeliveryRequest(OrderIdentifier, new Address("Address line 1", "TY6 7UI"));

            _ = request.ClaimDelivery("James");

            request.Driver.Should().Be("James");
            request.DriverCollectedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }
        
        [Fact]
        public async Task OrderReadyForDeliveryHandler_ShouldStoreNewDeliveryRequest()
        {
            var mockRepo = new Mock<IDeliveryRequestRepository>();
            mockRepo.Setup(p => p.AddNewDeliveryRequest(It.IsAny<DeliveryRequest>()))
                .Verifiable();
            var mockLogger = new Mock<IObservabilityService>();

            var handler = new OrderReadyForDeliveryEventHandler(mockRepo.Object, mockLogger.Object);

            await handler.Handle(new OrderReadyForDeliveryEventV1()
            {
                OrderIdentifier = "1234",
                DeliveryAddressLine1 = "AddressLine1",
                DeliveryAddressLine2 = "AddressLine2",
                DeliveryAddressLine3 = "AddressLine3",
                DeliveryAddressLine4 = "AddressLine4",
                DeliveryAddressLine5 = "AddressLine5",
                Postcode = "Postcode"
            });
            
            mockRepo.Verify(p => p.AddNewDeliveryRequest(It.IsAny<DeliveryRequest>()), Times.Once);
        }
        
        [Fact]
        public async Task OrderReadyForDeliveryHandlerImmutabilityCheck_ShouldSkipIfOrderAlreadyFound()
        {
            var mockRepo = new Mock<IDeliveryRequestRepository>();
            mockRepo.Setup(p => p.AddNewDeliveryRequest(It.IsAny<DeliveryRequest>()))
                .Verifiable();
            mockRepo.Setup(p => p.GetDeliveryStatusForOrder(It.IsAny<string>()))
                .ReturnsAsync(new DeliveryRequest(OrderIdentifier, new Address("Address line 1", "TY6 7UI")));
            
            var mockLogger = new Mock<IObservabilityService>();

            var handler = new OrderReadyForDeliveryEventHandler(mockRepo.Object, mockLogger.Object);

            await handler.Handle(new OrderReadyForDeliveryEventV1()
            {
                OrderIdentifier = "1234",
                DeliveryAddressLine1 = "AddressLine1",
                DeliveryAddressLine2 = "AddressLine2",
                DeliveryAddressLine3 = "AddressLine3",
                DeliveryAddressLine4 = "AddressLine4",
                DeliveryAddressLine5 = "AddressLine5",
                Postcode = "Postcode"
            });
            
            mockRepo.Verify(p => p.AddNewDeliveryRequest(It.IsAny<DeliveryRequest>()), Times.Never);
        }
    }
}