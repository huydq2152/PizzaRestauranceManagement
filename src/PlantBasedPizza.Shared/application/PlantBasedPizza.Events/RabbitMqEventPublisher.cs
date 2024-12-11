using System.Diagnostics;
using System.Text;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using JsonEventFormatter = CloudNative.CloudEvents.SystemTextJson.JsonEventFormatter;

namespace PlantBasedPizza.Events;

public class RabbitMqEventPublisher(
    IOptions<RabbitMqSettings> settings,
    ILogger<RabbitMqEventPublisher> logger,
    RabbitMqConnection connection)
    : IEventPublisher
{
    private readonly RabbitMqSettings _rabbitMqSettings = settings.Value;
    private readonly IConnection _connection = connection.Connection;

    public async Task Publish(IntegrationEvent evt)
    {
        var channel = await _connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: _rabbitMqSettings.ExchangeName, ExchangeType.Topic, durable: true);

        var queueName = $"{evt.EventName}.{evt.EventVersion}";

        await channel.QueueDeclareAsync(queueName, exclusive: false, durable: true);

        var eventId = Guid.NewGuid()
            .ToString();

        Activity.Current?.AddTag("messaging.eventId", eventId);
        Activity.Current?.AddTag("messaging.eventType", evt.EventName);
        Activity.Current?.AddTag("messaging.eventVersion", evt.EventVersion);
        Activity.Current?.AddTag("messagaing.eventSource", evt.Source);

        var evtWrapper = new CloudEvent
        {
            Type = queueName,
            Source = evt.Source,
            Time = DateTimeOffset.Now,
            DataContentType = "application/json",
            Id = eventId,
            Data = evt,
        };

        if (!string.IsNullOrEmpty(Activity.Current?.Id))
        {
            evtWrapper.SetAttributeFromString("traceparent", Activity.Current?.Id!);   
        }
        
        logger.LogInformation("Publishing event {EventId} {EventVersion} with traceId {TraceId}", evtWrapper.Id, evt.EventVersion, Activity.Current?.Id);

        var evtFormatter = new JsonEventFormatter();

        var json = evtFormatter.ConvertToJsonElement(evtWrapper).ToString();
        
        logger.LogInformation(json);
        
        var body = Encoding.UTF8.GetBytes(json);
        
        logger.LogInformation($"Publishing '{queueName}' to '{_rabbitMqSettings.ExchangeName}'");
        
        //put the data on to the product queue
        await channel.BasicPublishAsync(exchange: _rabbitMqSettings.ExchangeName, routingKey: $"{evt.EventName}.{evt.EventVersion}", body: body);
    }
}