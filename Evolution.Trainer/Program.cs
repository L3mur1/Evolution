using Evolution.Core;
using Evolution.Trainer;

const int maxTicks = 10_000;

try
{
    var config = WorldConfigLoader.Load();
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
            Console.WriteLine(
                $"Avg genes: metab={stats.AverageMetabolismGene:F3}, food={stats.AverageFoodGainGene:F3}, repro={stats.AverageReproductionThresholdGene:F3}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred while running the simulation:");
    Console.WriteLine(ex.Message);
}