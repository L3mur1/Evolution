using Evolution.Core;
using Evolution.Trainer;

try
{
    var config = WorldConfigLoader.Load();
    var world = new World(config);

    for (var i = 0; i < config.MaxTicks; i++)
    {
        world.Tick();

        if (world.Organisms.Count == 0)
        {
            var stats = world.GetStats();
            Console.WriteLine($"Tick: {stats.Tick}, Population extinct.");
            break;
        }

        var tick = world.TickNumber;
        if (tick > 0 && tick % 1000 == 0)
        {
            var stats = world.GetStats();
            Console.WriteLine(
                $"Tick: {stats.Tick}, Population: {stats.Population}, Average energy: {stats.AverageEnergy:F2}, Average age: {stats.AverageAge:F2}");
            Console.WriteLine(
                $"Avg genes: food={stats.AverageFoodGainGene:F3}, repro={stats.AverageReproductionThresholdGene:F3}, eyes={stats.AverageEyesGene:F3}, speed={stats.AverageSpeedGene:F3}");

            if (tick % 10_000 == 0)
            {
                Console.WriteLine(
                    $"Energy: avg={stats.AverageEnergy:F2}, min={stats.MinEnergy:F2}, max={stats.MaxEnergy:F2}, metabLoad={stats.AverageExtraMetabolicLoad:F4}");
                Console.WriteLine(
                    $"Age:    avg={stats.AverageAge:F0}, min={stats.MinAge}, max={stats.MaxAge}");
                Console.WriteLine(
                    $"Food  buckets: low={stats.FoodGainLowCount}, mid={stats.FoodGainMidCount}, high={stats.FoodGainHighCount}");
                Console.WriteLine(
                    $"Repro buckets: low={stats.ReproductionThresholdLowCount}, mid={stats.ReproductionThresholdMidCount}, high={stats.ReproductionThresholdHighCount}");
                Console.WriteLine(
                    $"Eyes  buckets: low={stats.EyesLowCount}, mid={stats.EyesMidCount}, high={stats.EyesHighCount}");
                Console.WriteLine(
                    $"Speed buckets: low={stats.SpeedLowCount}, mid={stats.SpeedMidCount}, high={stats.SpeedHighCount}");
                Console.WriteLine(
                    $"Sample (oldest): age={stats.SampleAge}, energy={stats.SampleEnergy:F2}, genes: food={stats.SampleFoodGainGene:F3}, repro={stats.SampleReproductionThresholdGene:F3}, eyes={stats.SampleEyesGene:F3}, speed={stats.SampleSpeedGene:F3}");
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred while running the simulation:");
    Console.WriteLine(ex.Message);
}