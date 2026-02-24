namespace Evolution.Core;

public sealed class WorldConfig
{
    public double EnergyFromFood { get; init; } = 5.0;
    public double EnergyLossPerTick { get; init; } = 0.2;
    public double FoodRegenProbability { get; init; } = 0.02;
    public int Height { get; init; } = 20;
    public double InitialFoodDensity { get; init; } = 0.1;
    public int InitialOrganismCount { get; init; } = 50;
    public int RandomSeed { get; init; } = 12345;
    public double ReproductionCost { get; init; } = 15.0;
    public double ReproductionThreshold { get; init; } = 40.0;
    public double StartEnergy { get; init; } = 20.0;
    public int Width { get; init; } = 40;
}