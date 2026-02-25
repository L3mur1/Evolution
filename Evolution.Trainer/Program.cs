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
        if (tick > 0 && tick % 10000 == 0)
        {
            var stats = world.GetStats();
            Console.WriteLine(
                $"Tick: {stats.Tick}, Population: {stats.Population}, Average energy: {stats.AverageEnergy:F2}, Average age: {stats.AverageAge:F2}");
            Console.WriteLine(
                $"Avg genes: food={stats.AverageFoodGainGene:F3}±{stats.FoodGainGeneStdDev:F3}, repro={stats.AverageReproductionThresholdGene:F3}±{stats.ReproductionThresholdGeneStdDev:F3}, eyes={stats.AverageEyesGene:F3}±{stats.EyesGeneStdDev:F3}, speed={stats.AverageSpeedGene:F3}±{stats.SpeedGeneStdDev:F3}");

            Console.WriteLine(
                               $"Energy: avg={stats.AverageEnergy:F2}, min={stats.MinEnergy:F2}, max={stats.MaxEnergy:F2}, metabLoad={stats.AverageExtraMetabolicLoad:F4}");
            Console.WriteLine(
                $"Age:    avg={stats.AverageAge:F0}, min={stats.MinAge}, max={stats.MaxAge}");
            Console.WriteLine(
                $"Oldest:      age={stats.OldestAge}, energy={stats.OldestEnergy:F2}, genes: food={stats.OldestFoodGainGene:F3}, repro={stats.OldestReproductionThresholdGene:F3}, eyes={stats.OldestEyesGene:F3}, speed={stats.OldestSpeedGene:F3}");
            Console.WriteLine(
                $"Best eyes:   age={stats.BestEyesAge}, energy={stats.BestEyesEnergy:F2}, genes: food={stats.BestEyesFoodGainGene:F3}, repro={stats.BestEyesReproductionThresholdGene:F3}, eyes={stats.BestEyesEyesGene:F3}, speed={stats.BestEyesSpeedGene:F3}");
            Console.WriteLine(
                $"Worst eyes:  age={stats.WorstEyesAge}, energy={stats.WorstEyesEnergy:F2}, genes: food={stats.WorstEyesFoodGainGene:F3}, repro={stats.WorstEyesReproductionThresholdGene:F3}, eyes={stats.WorstEyesEyesGene:F3}, speed={stats.WorstEyesSpeedGene:F3}");
            Console.WriteLine(
                $"Fastest:     age={stats.FastestAge}, energy={stats.FastestEnergy:F2}, genes: food={stats.FastestFoodGainGene:F3}, repro={stats.FastestReproductionThresholdGene:F3}, eyes={stats.FastestEyesGene:F3}, speed={stats.FastestSpeedGene:F3}");
            Console.WriteLine(
                $"Slowest:     age={stats.SlowestAge}, energy={stats.SlowestEnergy:F2}, genes: food={stats.SlowestFoodGainGene:F3}, repro={stats.SlowestReproductionThresholdGene:F3}, eyes={stats.SlowestEyesGene:F3}, speed={stats.SlowestSpeedGene:F3}");
            Console.WriteLine(
                $"Min energy:  age={stats.MinEnergyAge}, energy={stats.MinEnergyEnergy:F2}, genes: food={stats.MinEnergyFoodGainGene:F3}, repro={stats.MinEnergyReproductionThresholdGene:F3}, eyes={stats.MinEnergyEyesGene:F3}, speed={stats.MinEnergySpeedGene:F3}");
            Console.WriteLine(
                $"Max energy:  age={stats.MaxEnergyAge}, energy={stats.MaxEnergyEnergy:F2}, genes: food={stats.MaxEnergyFoodGainGene:F3}, repro={stats.MaxEnergyReproductionThresholdGene:F3}, eyes={stats.MaxEnergyEyesGene:F3}, speed={stats.MaxEnergySpeedGene:F3}");
            Console.WriteLine(
                $"Typical:     age={stats.TypicalAge}, energy={stats.TypicalEnergy:F2}, genes: food={stats.TypicalFoodGainGene:F3}, repro={stats.TypicalReproductionThresholdGene:F3}, eyes={stats.TypicalEyesGene:F3}, speed={stats.TypicalSpeedGene:F3}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred while running the simulation:");
    Console.WriteLine(ex.Message);
}