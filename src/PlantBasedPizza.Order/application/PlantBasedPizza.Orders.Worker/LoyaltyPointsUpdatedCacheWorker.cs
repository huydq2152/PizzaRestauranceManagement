using System.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using PlantBasedPizza.Events;
using PlantBasedPizza.Orders.Worker.IntegrationEvents;
using RabbitMQ.Client;

namespace PlantBasedPizza.Orders.Worker;

public class LoyaltyPointsUpdatedCacheWorker(
    RabbitMqEventSubscriber eventSubscriber,
    ActivitySource source,
    IDistributedCache distributedCache,
    ILogger<LoyaltyPointsUpdatedCacheWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = "orders-loyaltyPointsUpdated-worker";

        var subscription =
            await eventSubscriber.CreateEventConsumer(queueName, "loyalty.customerLoyaltyPointsUpdated.v1");

        subscription.Consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                logger.LogInformation("Processing message {messageId}", ea.DeliveryTag);

                var evtDataResponse =
                    await eventSubscriber.ParseEventFrom<CustomerLoyaltyPointsUpdatedEvent>(ea.Body.ToArray());

                using var processingActivity = source.StartActivity("processing-order-completed-event",
                    ActivityKind.Server, evtDataResponse.TraceParent);
                processingActivity.AddTag("queue.time", evtDataResponse.QueueTime);
                processingActivity.AddTag("message.id", ea.DeliveryTag);
                processingActivity.AddTag("customerIdentifier", evtDataResponse.EventData.CustomerIdentifier);
                processingActivity.AddTag("totalPoints", evtDataResponse.EventData.TotalLoyaltyPoints);

                await distributedCache.SetStringAsync(evtDataResponse.EventData.CustomerIdentifier.ToUpper(),
                    evtDataResponse.EventData.TotalLoyaltyPoints.ToString("n0"), stoppingToken);

                logger.LogInformation("Cached");

                await subscription.Channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failure processing message");

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