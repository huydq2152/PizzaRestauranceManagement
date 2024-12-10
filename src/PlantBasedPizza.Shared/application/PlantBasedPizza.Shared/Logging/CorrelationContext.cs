namespace PlantBasedPizza.Shared.Logging;

public static class CorrelationContext
{
    private static readonly AsyncLocal<string> CorrelationId = new AsyncLocal<string>();

    private const string __defaultRequestHeaderName = "X-Correlation-ID";

    public static string DefaultRequestHeaderName => __defaultRequestHeaderName;
    
    public static void SetCorrelationId(string correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            throw new ArgumentException("Correlation Id cannot be null or empty", nameof(correlationId));
        }

        if (!string.IsNullOrWhiteSpace(CorrelationId.Value))
        {
            throw new InvalidOperationException("Correlation Id is already set for the context");
        }

        CorrelationId.Value = correlationId;
    }

    public static string GetCorrelationId()
    {
        return CorrelationId.Value ?? string.Empty;
    }
}