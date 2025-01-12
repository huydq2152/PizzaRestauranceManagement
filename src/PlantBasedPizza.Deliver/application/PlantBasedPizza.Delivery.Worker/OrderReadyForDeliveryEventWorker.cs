using System.Diagnostics;
using PlantBasedPizza.Delivery.Core.Handlers;
using PlantBasedPizza.Delivery.Core.IntegrationEvents;
using PlantBasedPizza.Events;
using RabbitMQ.Client;

namespace PlantBasedPizza.Delivery.Worker;

public class OrderReadyForDeliveryEventWorker(
    RabbitMqEventSubscriber eventSubscriber,
    ActivitySource source,
    OrderReadyForDeliveryEventHandler eventHandler)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = "delivery-readyForDelivery-worker";

        var subscription = await eventSubscriber.CreateEventConsumer(queueName, "order.readyForDelivery.v1");
        
        subscription.Consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var evtDataResponse = await eventSubscriber.ParseEventFrom<OrderReadyForDeliveryEventV1>(ea.Body.ToArray());

                using var processingActivity = source.StartActivity("processing-order-quality-checked-event",
                    ActivityKind.Server, evtDataResponse.TraceParent);
                processingActivity.AddTag("queue.time", evtDataResponse.QueueTime);

                processingActivity.SetTag("orderIdentifier", evtDataResponse.EventData.OrderIdentifier);

                await eventHandler.Handle(evtDataResponse.EventData);
            
                await subscription.Channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception e)
            {
                await subscription.Channel.BasicRejectAsync(ea.DeliveryTag, true, stoppingToken);
            }
        };
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await subscription.Channel.BasicConsumeAsync(
                queueName,
                false,
                subscription.Consumer, stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}