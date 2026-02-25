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
    public BiomeMapConfig? BiomeMap { get; init; }

    /// <summary>
    /// Preferred radius (in tiles) around an organism's home within which movement
    /// does not incur additional home-range energy costs. Set to 0 or negative to disable.
    /// </summary>
    public double HomeRadius { get; init; }

    /// <summary>
    /// Additional energy cost per tile of distance beyond HomeRadius.
    /// Set to 0 to disable distance-from-home penalties.
    /// </summary>
    public double HomeFarCostPerTile { get; init; }

    /// <summary>
    /// Number of consecutive ticks an organism must spend outside its home radius
    /// before its home begins to migrate toward its current position.
    /// Set to 0 to disable automatic home relocation.
    /// </summary>
    public int HomeRelocationThresholdTicks { get; init; }

    /// <summary>
    /// Fractional factor controlling how quickly an organism's home moves toward
    /// its current position once relocation is triggered. Values in (0, 1] are typical.
    /// Set to 0 to disable relocation or when using a discrete step-based relocation scheme.
    /// </summary>
    public double HomeRelocationFactor { get; init; }

    /// <summary>
    /// Scale factor controlling how strongly the HomeRelocationGene modifies the
    /// configured HomeRelocationFactor. Positive values make relocation faster for
    /// positive genes and slower for negative genes.
    /// </summary>
    public double HomeRelocationGeneScale { get; init; }

    /// <summary>
    /// Scale factor controlling how strongly the WanderGene modifies the configured
    /// HomeFarCostPerTile. Positive values reduce the distance-from-home penalty for
    /// positive genes (more nomadic) and increase it for negative genes (more home-bound).
    /// </summary>
    public double WanderGeneScale { get; init; }

    /// <summary>
    /// Quadratic energy cost coefficients for HomeRelocationGene.
    /// Positive/negative sides can be tuned independently (like for eyes/speed).
    /// </summary>
    public double HomeRelocationGeneCostPositive { get; init; }
    public double HomeRelocationGeneCostNegative { get; init; }

    /// <summary>
    /// Quadratic energy cost coefficients for WanderGene.
    /// Positive/negative sides can be tuned independently (like for eyes/speed).
    /// </summary>
    public double WanderGeneCostPositive { get; init; }
    public double WanderGeneCostNegative { get; init; }
}