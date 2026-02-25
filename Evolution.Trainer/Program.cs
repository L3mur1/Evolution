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
                $"Oldest:      age={stats.OldestAge}, energy={stats.OldestEnergy:F2}, genes: " +
                $"food={stats.OldestFoodGainGene:F3} (Δ={(stats.OldestFoodGainGene - stats.AverageFoodGainGene):+0.000;-0.000;+0.000}), " +
                $"repro={stats.OldestReproductionThresholdGene:F3} (Δ={(stats.OldestReproductionThresholdGene - stats.AverageReproductionThresholdGene):+0.000;-0.000;+0.000}), " +
                $"eyes={stats.OldestEyesGene:F3} (Δ={(stats.OldestEyesGene - stats.AverageEyesGene):+0.000;-0.000;+0.000}), " +
                $"speed={stats.OldestSpeedGene:F3} (Δ={(stats.OldestSpeedGene - stats.AverageSpeedGene):+0.000;-0.000;+0.000})");
            Console.WriteLine(
                $"Best eyes:   age={stats.BestEyesAge}, energy={stats.BestEyesEnergy:F2}, genes: " +
                $"food={stats.BestEyesFoodGainGene:F3} (Δ={(stats.BestEyesFoodGainGene - stats.AverageFoodGainGene):+0.000;-0.000;+0.000}), " +
                $"repro={stats.BestEyesReproductionThresholdGene:F3} (Δ={(stats.BestEyesReproductionThresholdGene - stats.AverageReproductionThresholdGene):+0.000;-0.000;+0.000}), " +
                $"eyes={stats.BestEyesEyesGene:F3} (Δ={(stats.BestEyesEyesGene - stats.AverageEyesGene):+0.000;-0.000;+0.000}), " +
                $"speed={stats.BestEyesSpeedGene:F3} (Δ={(stats.BestEyesSpeedGene - stats.AverageSpeedGene):+0.000;-0.000;+0.000})");
            Console.WriteLine(
                $"Worst eyes:  age={stats.WorstEyesAge}, energy={stats.WorstEyesEnergy:F2}, genes: " +
                $"food={stats.WorstEyesFoodGainGene:F3} (Δ={(stats.WorstEyesFoodGainGene - stats.AverageFoodGainGene):+0.000;-0.000;+0.000}), " +
                $"repro={stats.WorstEyesReproductionThresholdGene:F3} (Δ={(stats.WorstEyesReproductionThresholdGene - stats.AverageReproductionThresholdGene):+0.000;-0.000;+0.000}), " +
                $"eyes={stats.WorstEyesEyesGene:F3} (Δ={(stats.WorstEyesEyesGene - stats.AverageEyesGene):+0.000;-0.000;+0.000}), " +
                $"speed={stats.WorstEyesSpeedGene:F3} (Δ={(stats.WorstEyesSpeedGene - stats.AverageSpeedGene):+0.000;-0.000;+0.000})");
            Console.WriteLine(
                $"Fastest:     age={stats.FastestAge}, energy={stats.FastestEnergy:F2}, genes: " +
                $"food={stats.FastestFoodGainGene:F3} (Δ={(stats.FastestFoodGainGene - stats.AverageFoodGainGene):+0.000;-0.000;+0.000}), " +
                $"repro={stats.FastestReproductionThresholdGene:F3} (Δ={(stats.FastestReproductionThresholdGene - stats.AverageReproductionThresholdGene):+0.000;-0.000;+0.000}), " +
                $"eyes={stats.FastestEyesGene:F3} (Δ={(stats.FastestEyesGene - stats.AverageEyesGene):+0.000;-0.000;+0.000}), " +
                $"speed={stats.FastestSpeedGene:F3} (Δ={(stats.FastestSpeedGene - stats.AverageSpeedGene):+0.000;-0.000;+0.000})");
            Console.WriteLine(
                $"Slowest:     age={stats.SlowestAge}, energy={stats.SlowestEnergy:F2}, genes: " +
                $"food={stats.SlowestFoodGainGene:F3} (Δ={(stats.SlowestFoodGainGene - stats.AverageFoodGainGene):+0.000;-0.000;+0.000}), " +
                $"repro={stats.SlowestReproductionThresholdGene:F3} (Δ={(stats.SlowestReproductionThresholdGene - stats.AverageReproductionThresholdGene):+0.000;-0.000;+0.000}), " +
                $"eyes={stats.SlowestEyesGene:F3} (Δ={(stats.SlowestEyesGene - stats.AverageEyesGene):+0.000;-0.000;+0.000}), " +
                $"speed={stats.SlowestSpeedGene:F3} (Δ={(stats.SlowestSpeedGene - stats.AverageSpeedGene):+0.000;-0.000;+0.000})");
            Console.WriteLine(
                $"Min energy:  age={stats.MinEnergyAge}, energy={stats.MinEnergyEnergy:F2}, genes: " +
                $"food={stats.MinEnergyFoodGainGene:F3} (Δ={(stats.MinEnergyFoodGainGene - stats.AverageFoodGainGene):+0.000;-0.000;+0.000}), " +
                $"repro={stats.MinEnergyReproductionThresholdGene:F3} (Δ={(stats.MinEnergyReproductionThresholdGene - stats.AverageReproductionThresholdGene):+0.000;-0.000;+0.000}), " +
                $"eyes={stats.MinEnergyEyesGene:F3} (Δ={(stats.MinEnergyEyesGene - stats.AverageEyesGene):+0.000;-0.000;+0.000}), " +
                $"speed={stats.MinEnergySpeedGene:F3} (Δ={(stats.MinEnergySpeedGene - stats.AverageSpeedGene):+0.000;-0.000;+0.000})");
            Console.WriteLine(
                $"Max energy:  age={stats.MaxEnergyAge}, energy={stats.MaxEnergyEnergy:F2}, genes: " +
                $"food={stats.MaxEnergyFoodGainGene:F3} (Δ={(stats.MaxEnergyFoodGainGene - stats.AverageFoodGainGene):+0.000;-0.000;+0.000}), " +
                $"repro={stats.MaxEnergyReproductionThresholdGene:F3} (Δ={(stats.MaxEnergyReproductionThresholdGene - stats.AverageReproductionThresholdGene):+0.000;-0.000;+0.000}), " +
                $"eyes={stats.MaxEnergyEyesGene:F3} (Δ={(stats.MaxEnergyEyesGene - stats.AverageEyesGene):+0.000;-0.000;+0.000}), " +
                $"speed={stats.MaxEnergySpeedGene:F3} (Δ={(stats.MaxEnergySpeedGene - stats.AverageSpeedGene):+0.000;-0.000;+0.000})");
            Console.WriteLine(
                $"Typical:     age={stats.TypicalAge}, energy={stats.TypicalEnergy:F2}, genes: " +
                $"food={stats.TypicalFoodGainGene:F3} (Δ={(stats.TypicalFoodGainGene - stats.AverageFoodGainGene):+0.000;-0.000;+0.000}), " +
                $"repro={stats.TypicalReproductionThresholdGene:F3} (Δ={(stats.TypicalReproductionThresholdGene - stats.AverageReproductionThresholdGene):+0.000;-0.000;+0.000}), " +
                $"eyes={stats.TypicalEyesGene:F3} (Δ={(stats.TypicalEyesGene - stats.AverageEyesGene):+0.000;-0.000;+0.000}), " +
                $"speed={stats.TypicalSpeedGene:F3} (Δ={(stats.TypicalSpeedGene - stats.AverageSpeedGene):+0.000;-0.000;+0.000})");

            if (stats.BiomeStats.Count > 0)
            {
                Console.WriteLine("Biome stats:");
                foreach (var biome in stats.BiomeStats)
                {
                    if (biome.Population == 0)
                    {
                        continue;
                    }

                    Console.WriteLine(
                        $"  Biome {biome.BiomeName}: cells={biome.CellCount}, pop={biome.Population}, density={biome.Density:F3}, " +
                        $"avgEnergy={biome.AverageEnergy:F2}, avgAge={biome.AverageAge:F0}, " +
                        $"genes: food={biome.AverageFoodGainGene:F3}, repro={biome.AverageReproductionThresholdGene:F3}, eyes={biome.AverageEyesGene:F3}, speed={biome.AverageSpeedGene:F3}, " +
                        $"food/cell={biome.AverageFoodPerCell:F3}, birthsLastTick={biome.BirthsLastTick}, deathsLastTick={biome.DeathsLastTick}");
                }
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred while running the simulation:");
    Console.WriteLine(ex.Message);
}