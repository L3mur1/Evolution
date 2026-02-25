namespace Evolution.Core;

public sealed class WorldStats
{
    public double AverageAge { get; init; }
    public double AverageEnergy { get; init; }
    public int Population { get; init; }
    public int Tick { get; init; }

    public double AverageFoodGainGene { get; init; }
    public double AverageReproductionThresholdGene { get; init; }
    public double AverageEyesGene { get; init; }
    public double AverageSpeedGene { get; init; }

    // Console observability – scalar aggregates
    public double MinEnergy { get; init; }
    public double MaxEnergy { get; init; }
    public int MinAge { get; init; }
    public int MaxAge { get; init; }

    // Gene buckets (low/mid/high) for food gain and reproduction threshold
    public int FoodGainLowCount { get; init; }
    public int FoodGainMidCount { get; init; }
    public int FoodGainHighCount { get; init; }

    public int ReproductionThresholdLowCount { get; init; }
    public int ReproductionThresholdMidCount { get; init; }
    public int ReproductionThresholdHighCount { get; init; }

    public int EyesLowCount { get; init; }
    public int EyesMidCount { get; init; }
    public int EyesHighCount { get; init; }

    public int SpeedLowCount { get; init; }
    public int SpeedMidCount { get; init; }
    public int SpeedHighCount { get; init; }

    // Optional sample organism snapshot (oldest individual)
    public double SampleEnergy { get; init; }
    public int SampleAge { get; init; }
    public double SampleFoodGainGene { get; init; }
    public double SampleReproductionThresholdGene { get; init; }
    public double SampleEyesGene { get; init; }
    public double SampleSpeedGene { get; init; }
}