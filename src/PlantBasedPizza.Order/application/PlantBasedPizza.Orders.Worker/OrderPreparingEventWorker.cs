using System.Diagnostics;
using PlantBasedPizza.Events;
using PlantBasedPizza.Orders.Worker.Handlers;
using PlantBasedPizza.Orders.Worker.IntegrationEvents;
using RabbitMQ.Client;

namespace PlantBasedPizza.Orders.Worker;

public class OrderPreparingEventWorker(
    RabbitMqEventSubscriber eventSubscriber,
    ActivitySource source,
    OrderPreparingEventHandler eventHandler)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = "orders-orderPreparing-worker";

        var subscription = await eventSubscriber.CreateEventConsumer(queueName, "kitchen.orderPreparing.v1");

        subscription.Consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var evtDataResponse = await eventSubscriber.ParseEventFrom<OrderPreparingEventV1>(ea.Body.ToArray());

                using var processingActivity = source.StartActivity("processing-order-preparing-event",
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