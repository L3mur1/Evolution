namespace Evolution.Core;

public sealed class WorldConfig
{
    public double EnergyFromFood { get; init; }
    public double EnergyLossPerTick { get; init; }
    public double EyesBaseRadius { get; init; }
    public double EyesEnergyCostNegative { get; init; }
    public double EyesEnergyCostPositive { get; init; }
    public double EyesRadiusScale { get; init; }
    public double FoodGainBase { get; init; }
    public double FoodGainGeneScale { get; init; }
    public double FoodGainPenaltyNegative { get; init; }
    public double FoodGainPenaltyPositive { get; init; }
    public double FoodRegenProbability { get; init; }
    public int Height { get; init; }
    public double InitialFoodDensity { get; init; }
    public int InitialOrganismCount { get; init; }
    public int MaxTicks { get; init; }
    public double MetabolismBase { get; init; }
    public double MutationProbabilityPerGene { get; init; }
    public double MutationStepSize { get; init; }
    public double OldAgeEnergyThreshold { get; init; }
    public double OldAgeLowEnergyDeathProbability { get; init; }
    public int OldAgeThreshold { get; init; }
    public int RandomSeed { get; init; }
    public double ReproductionCost { get; init; }
    public double ReproductionThreshold { get; init; }
    public double ReproductionThresholdBase { get; init; }
    public double ReproductionThresholdGeneScale { get; init; }
    public double SpeedBaseTwoStepChance { get; init; }
    public double SpeedChanceScale { get; init; }
    public double SpeedExtraStepCost { get; init; }
    public double SpeedGeneCostNegative { get; init; }
    public double SpeedGeneCostPositive { get; init; }
    public double StartEnergy { get; init; }
    public int Width { get; init; }
    public BiomeConfig Biome { get; init; } = new();
}