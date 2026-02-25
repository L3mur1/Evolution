using System;
using System.Collections.Generic;

namespace Evolution.Core;

public sealed class WorldStats
{
    public double AverageAge { get; init; }
    public double AverageEnergy { get; init; }
    public double AverageExtraMetabolicLoad { get; init; }
    public double AverageEyesGene { get; init; }
    public double AverageFoodGainGene { get; init; }
    public double AverageReproductionThresholdGene { get; init; }
    public double AverageSpeedGene { get; init; }
    public double AverageHomeRelocationGene { get; init; }
    public double AverageWanderGene { get; init; }
    public double FoodGainGeneStdDev { get; init; }
    public double ReproductionThresholdGeneStdDev { get; init; }
    public double EyesGeneStdDev { get; init; }
    public double SpeedGeneStdDev { get; init; }
    public double HomeRelocationGeneStdDev { get; init; }
    public double WanderGeneStdDev { get; init; }
    public int MaxAge { get; init; }
    public double MaxEnergy { get; init; }
    public int MinAge { get; init; }
    public double MinEnergy { get; init; }
    public int Population { get; init; }
    public int OldestAge { get; init; }
    public double OldestEnergy { get; init; }
    public double OldestFoodGainGene { get; init; }
    public double OldestReproductionThresholdGene { get; init; }
    public double OldestEyesGene { get; init; }
    public double OldestSpeedGene { get; init; }
    public double OldestHomeRelocationGene { get; init; }
    public double OldestWanderGene { get; init; }

    public int BestEyesAge { get; init; }
    public double BestEyesEnergy { get; init; }
    public double BestEyesFoodGainGene { get; init; }
    public double BestEyesReproductionThresholdGene { get; init; }
    public double BestEyesEyesGene { get; init; }
    public double BestEyesSpeedGene { get; init; }
    public double BestEyesHomeRelocationGene { get; init; }
    public double BestEyesWanderGene { get; init; }

    public int WorstEyesAge { get; init; }
    public double WorstEyesEnergy { get; init; }
    public double WorstEyesFoodGainGene { get; init; }
    public double WorstEyesReproductionThresholdGene { get; init; }
    public double WorstEyesEyesGene { get; init; }
    public double WorstEyesSpeedGene { get; init; }
    public double WorstEyesHomeRelocationGene { get; init; }
    public double WorstEyesWanderGene { get; init; }

    public int FastestAge { get; init; }
    public double FastestEnergy { get; init; }
    public double FastestFoodGainGene { get; init; }
    public double FastestReproductionThresholdGene { get; init; }
    public double FastestEyesGene { get; init; }
    public double FastestSpeedGene { get; init; }
    public double FastestHomeRelocationGene { get; init; }
    public double FastestWanderGene { get; init; }

    public int SlowestAge { get; init; }
    public double SlowestEnergy { get; init; }
    public double SlowestFoodGainGene { get; init; }
    public double SlowestReproductionThresholdGene { get; init; }
    public double SlowestEyesGene { get; init; }
    public double SlowestSpeedGene { get; init; }
    public double SlowestHomeRelocationGene { get; init; }
    public double SlowestWanderGene { get; init; }

    public int MinEnergyAge { get; init; }
    public double MinEnergyEnergy { get; init; }
    public double MinEnergyFoodGainGene { get; init; }
    public double MinEnergyReproductionThresholdGene { get; init; }
    public double MinEnergyEyesGene { get; init; }
    public double MinEnergySpeedGene { get; init; }
    public double MinEnergyHomeRelocationGene { get; init; }
    public double MinEnergyWanderGene { get; init; }

    public int MaxEnergyAge { get; init; }
    public double MaxEnergyEnergy { get; init; }
    public double MaxEnergyFoodGainGene { get; init; }
    public double MaxEnergyReproductionThresholdGene { get; init; }
    public double MaxEnergyEyesGene { get; init; }
    public double MaxEnergySpeedGene { get; init; }
    public double MaxEnergyHomeRelocationGene { get; init; }
    public double MaxEnergyWanderGene { get; init; }

    public int TypicalAge { get; init; }
    public double TypicalEnergy { get; init; }
    public double TypicalFoodGainGene { get; init; }
    public double TypicalReproductionThresholdGene { get; init; }
    public double TypicalEyesGene { get; init; }
    public double TypicalSpeedGene { get; init; }
    public double TypicalHomeRelocationGene { get; init; }
    public double TypicalWanderGene { get; init; }

    public int Tick { get; init; }

    public IReadOnlyList<BiomeStats> BiomeStats { get; init; } = [];
}

public sealed class BiomeStats
{
    public string BiomeName { get; init; } = string.Empty;

    public int CellCount { get; init; }
    public int Population { get; init; }

    public double Density { get; init; }

    public double AverageEnergy { get; init; }
    public double AverageAge { get; init; }

    public double AverageFoodGainGene { get; init; }
    public double AverageReproductionThresholdGene { get; init; }
    public double AverageEyesGene { get; init; }
    public double AverageSpeedGene { get; init; }
    public double AverageHomeRelocationGene { get; init; }
    public double AverageWanderGene { get; init; }

    public double AverageFoodPerCell { get; init; }

    public int BirthsLastTick { get; init; }
    public int DeathsLastTick { get; init; }
}