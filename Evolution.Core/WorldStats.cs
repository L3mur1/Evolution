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
    public double FoodGainGeneStdDev { get; init; }
    public double ReproductionThresholdGeneStdDev { get; init; }
    public double EyesGeneStdDev { get; init; }
    public double SpeedGeneStdDev { get; init; }
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

    public int BestEyesAge { get; init; }
    public double BestEyesEnergy { get; init; }
    public double BestEyesFoodGainGene { get; init; }
    public double BestEyesReproductionThresholdGene { get; init; }
    public double BestEyesEyesGene { get; init; }
    public double BestEyesSpeedGene { get; init; }

    public int WorstEyesAge { get; init; }
    public double WorstEyesEnergy { get; init; }
    public double WorstEyesFoodGainGene { get; init; }
    public double WorstEyesReproductionThresholdGene { get; init; }
    public double WorstEyesEyesGene { get; init; }
    public double WorstEyesSpeedGene { get; init; }

    public int FastestAge { get; init; }
    public double FastestEnergy { get; init; }
    public double FastestFoodGainGene { get; init; }
    public double FastestReproductionThresholdGene { get; init; }
    public double FastestEyesGene { get; init; }
    public double FastestSpeedGene { get; init; }

    public int SlowestAge { get; init; }
    public double SlowestEnergy { get; init; }
    public double SlowestFoodGainGene { get; init; }
    public double SlowestReproductionThresholdGene { get; init; }
    public double SlowestEyesGene { get; init; }
    public double SlowestSpeedGene { get; init; }

    public int MinEnergyAge { get; init; }
    public double MinEnergyEnergy { get; init; }
    public double MinEnergyFoodGainGene { get; init; }
    public double MinEnergyReproductionThresholdGene { get; init; }
    public double MinEnergyEyesGene { get; init; }
    public double MinEnergySpeedGene { get; init; }

    public int MaxEnergyAge { get; init; }
    public double MaxEnergyEnergy { get; init; }
    public double MaxEnergyFoodGainGene { get; init; }
    public double MaxEnergyReproductionThresholdGene { get; init; }
    public double MaxEnergyEyesGene { get; init; }
    public double MaxEnergySpeedGene { get; init; }

    public int TypicalAge { get; init; }
    public double TypicalEnergy { get; init; }
    public double TypicalFoodGainGene { get; init; }
    public double TypicalReproductionThresholdGene { get; init; }
    public double TypicalEyesGene { get; init; }
    public double TypicalSpeedGene { get; init; }

    public int Tick { get; init; }
}