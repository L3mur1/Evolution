using Evolution.Core;

const int maxTicks = 10_000;

var config = new WorldConfig
{
    Width = 40,
    Height = 20,
    InitialFoodDensity = 0.1,
    FoodRegenProbability = 0.02,
    InitialOrganismCount = 50,
    StartEnergy = 20.0,
    EnergyLossPerTick = 0.2,
    EnergyFromFood = 5.0,
    ReproductionThreshold = 40.0,
    ReproductionCost = 15.0,
    RandomSeed = 12345
};

var world = new World(config);

for (var i = 0; i < maxTicks; i++)
{
    world.Tick();

    if (world.Organisms.Count == 0)
    {
        var stats = world.GetStats();
        Console.WriteLine($"Tick: {stats.Tick}, Population extinct.");
        break;
    }

    if (world.TickNumber % 100 == 0)
    {
        var stats = world.GetStats();
        Console.WriteLine(
            $"Tick: {stats.Tick}, Population: {stats.Population}, Average energy: {stats.AverageEnergy:F2}, Average age: {stats.AverageAge:F2}");
    }
}

