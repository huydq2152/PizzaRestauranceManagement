using System;

namespace PlantBasedPizza.IntegrationTests.Drivers;

public static class TestConstants
{
    public static readonly string DefaultTestUrl = Environment.GetEnvironmentVariable("TEST_URL") ?? "http://localhost:5051";
}