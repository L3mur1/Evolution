namespace Evolution.Core;

public sealed class WorldConfig
{
    public double EnergyFromFood { get; init; } = 5.0;
    public double EnergyLossPerTick { get; init; } = 0.2;
    public double EyesBaseRadius { get; init; } = 1.0;
    public double EyesEnergyCostCoefficient { get; init; } = 0.002;
    public double EyesRadiusScale { get; init; } = 2.0;
    public double FoodGainBase { get; init; } = 5.0;
    public double FoodGainGeneScale { get; init; } = 0.3;
    public double FoodRegenProbability { get; init; } = 0.02;
    public int Height { get; init; } = 20;
    public int MaxTicks { get; init; } = 100_000;
    public double InitialFoodDensity { get; init; } = 0.1;
    public int InitialOrganismCount { get; init; } = 50;
    public double MetabolismBase { get; init; } = 0.2;
    public double MutationProbabilityPerGene { get; init; } = 0.05;
    public double MutationStepSize { get; init; } = 0.1;
    public double OldAgeEnergyThreshold { get; init; } = 10.0;
    public double OldAgeLowEnergyDeathProbability { get; init; } = 0.08;
    public int OldAgeThreshold { get; init; } = 1000;
    public int RandomSeed { get; init; } = 12345;
    public double SpeedChanceScale { get; init; } = 0.5;
    public double SpeedExtraStepCost { get; init; } = 0.05;
    public double SpeedGeneCostCoefficient { get; init; } = 0.01;
    public double FoodGainPenaltyCoefficient { get; init; } = 0.02;
    public double ReproductionCost { get; init; } = 15.0;
    public double ReproductionThreshold { get; init; } = 40.0;
    public double ReproductionThresholdBase { get; init; } = 40.0;
    public double ReproductionThresholdGeneScale { get; init; } = 0.3;
    public double StartEnergy { get; init; } = 20.0;
    public int Width { get; init; } = 40;
}