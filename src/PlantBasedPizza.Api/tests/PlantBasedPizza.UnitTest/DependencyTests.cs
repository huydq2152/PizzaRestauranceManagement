using System.Collections.Generic;
using FluentAssertions;
using NetArchTest.Rules;
using PlantBasedPizza.Delivery.Core.Entities;
using PlantBasedPizza.Kitchen.Core.Entities;
using PlantBasedPizza.Recipes.Core.Entities;
using Xunit;

namespace PlantBasedPizza.UnitTest;

public class DependencyTests
{
    [Fact]
    public void OrderManagerDependencyTests()
    {
        var namespacesToCheck = new List<string>(6)
        {
            "PlantBasedPizza.Recipes.Core",
            "PlantBasedPizza.Recipes.Infrastructure",
            "PlantBasedPizza.Kitchen.Core",
            "PlantBasedPizza.Kitchen.Infrastructure",
            "PlantBasedPizza.Delivery.Core",
            "PlantBasedPizza.Delivery.Infrastructure",
        };

        foreach (var namespaceToCheck in namespacesToCheck)
        {
            Types.InAssembly(typeof(Order.Core.Entities.Order).Assembly)
                .ShouldNot()
                .HaveDependencyOn(namespaceToCheck)
                .GetResult().IsSuccessful.Should().BeTrue($"{namespaceToCheck} cannot be referenced.");   
        }
    }
    [Fact]
    public void KitchenDependencyTests()
    {
        var namespacesToCheck = new List<string>(6)
        {
            "PlantBasedPizza.Recipes.Core",
            "PlantBasedPizza.Recipes.Infrastructure",
            "PlantBasedPizza.Order.Core",
            "PlantBasedPizza.Order.Infrastructure",
            "PlantBasedPizza.Delivery.Core",
            "PlantBasedPizza.Delivery.Infrastructure",
        };

        foreach (var namespaceToCheck in namespacesToCheck)
        {
            Types.InAssembly(typeof(KitchenRequest).Assembly)
                .ShouldNot()
                .HaveDependencyOn(namespaceToCheck)
                .GetResult().IsSuccessful.Should().BeTrue($"{namespaceToCheck} cannot be referenced.");   
        }
    }
        
    [Fact]
    public void RecipesDependencyTests()
    {
        var namespacesToCheck = new List<string>(6)
        {
            "PlantBasedPizza.Order.Core",
            "PlantBasedPizza.Order.Infrastructure",
            "PlantBasedPizza.Kitchen.Core",
            "PlantBasedPizza.Kitchen.Infrastructure",
            "PlantBasedPizza.Delivery.Core",
            "PlantBasedPizza.Delivery.Infrastructure",
        };

        foreach (var namespaceToCheck in namespacesToCheck)
        {
            Types.InAssembly(typeof(Recipe).Assembly)
                .ShouldNot()
                .HaveDependencyOn(namespaceToCheck)
                .GetResult().IsSuccessful.Should().BeTrue($"{namespaceToCheck} cannot be referenced.");   
        }
    }
    [Fact]
    public void DeliveryDependencyTests()
    {
        var namespacesToCheck = new List<string>(6)
        {
            "PlantBasedPizza.Recipes.Core",
            "PlantBasedPizza.Recipes.Infrastructure",
            "PlantBasedPizza.Kitchen.Core",
            "PlantBasedPizza.Kitchen.Infrastructure",
            "PlantBasedPizza.Order.Core",
            "PlantBasedPizza.Order.Infrastructure",
        };

        foreach (var namespaceToCheck in namespacesToCheck)
        {
            Types.InAssembly(typeof(DeliveryRequest).Assembly)
                .ShouldNot()
                .HaveDependencyOn(namespaceToCheck)
                .GetResult().IsSuccessful.Should().BeTrue($"{namespaceToCheck} cannot be referenced.");   
        }
    }
}