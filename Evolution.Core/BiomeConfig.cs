namespace Evolution.Core;

public sealed class BiomeConfig
{
    public string Name { get; init; } = string.Empty;
    public double FoodInitialDensityMultiplier { get; init; } = 1.0;
    public double FoodRegenMultiplier { get; init; } = 1.0;
    public double MetabolismMultiplier { get; init; } = 1.0;
    public double FoodGainMultiplier { get; init; } = 1.0;
}

