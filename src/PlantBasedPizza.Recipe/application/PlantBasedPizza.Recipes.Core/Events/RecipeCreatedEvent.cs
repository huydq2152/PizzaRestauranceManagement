using PlantBasedPizza.Events;
using PlantBasedPizza.Recipes.Core.Entities;

namespace PlantBasedPizza.Recipes.Core.Events
{
    public class RecipeCreatedEvent(Recipe recipe, string correlationId) : IDomainEvent
    {
        public string EventName => "recipes.recipe-created";
        
        public string EventVersion => "v1";
        public string EventId { get; } = Guid.NewGuid().ToString();
        public DateTime EventDate { get; } = DateTime.Now;
        public string CorrelationId { get; set; } = correlationId;
        public Recipe Recipe { get; } = recipe;
    }
}