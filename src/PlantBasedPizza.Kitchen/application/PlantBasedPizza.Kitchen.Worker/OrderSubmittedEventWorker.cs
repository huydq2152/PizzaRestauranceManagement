using System.Diagnostics;
using PlantBasedPizza.Events;
using PlantBasedPizza.Kitchen.Worker.Handlers;
using PlantBasedPizza.Kitchen.Worker.IntegrationEvents;
using RabbitMQ.Client;

namespace PlantBasedPizza.Kitchen.Worker;

public class OrderSubmittedEventWorker(
    RabbitMqEventSubscriber eventSubscriber,
    ActivitySource source,
    OrderSubmittedEventHandler eventHandler,
    ILogger<OrderSubmittedEventWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting worker");

        var queueName = "kitchen-orderSubmitted-worker";

        var subscription = await eventSubscriber.CreateEventConsumer(queueName, "order.orderSubmitted.v1");

        subscription.Consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                logger.LogInformation("Received event, processing");

                var evtDataResponse = await eventSubscriber.ParseEventFrom<OrderSubmittedEventV1>(ea.Body.ToArray());

                using var processingActivity = source.StartActivity("kitchen-process-order-submitted-event",
                    ActivityKind.Server, evtDataResponse.TraceParent);
                processingActivity.AddTag("queue.time", evtDataResponse.QueueTime);

                processingActivity.SetTag("orderIdentifier", evtDataResponse.EventData.OrderIdentifier);

                logger.LogInformation($"Event is for order {evtDataResponse.EventData.OrderIdentifier}");

                await eventHandler.Handle(evtDataResponse.EventData);
                await subscription.Channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Received event, processing");

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