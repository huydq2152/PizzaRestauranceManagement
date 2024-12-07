using Newtonsoft.Json;

namespace PlantBasedPizza.OrderManager.Core.Entities;

public class OrderHistory(string description, DateTime historyDate)
{
    [JsonConstructor]
    private OrderHistory() : this("", new DateTime())
    {
    }

    [JsonProperty]
    public string Description { get; private set; } = description;

    [JsonProperty]
    public DateTime HistoryDate { get; private set; } = historyDate;
}