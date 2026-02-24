namespace Evolution.Core;

public sealed class WorldStats
{
    public double AverageAge { get; init; }
    public double AverageEnergy { get; init; }
    public int Population { get; init; }
    public int Tick { get; init; }

    // Stage 2 – genome trait summaries
    public double AverageMetabolismGene { get; init; }
    public double AverageFoodGainGene { get; init; }
    public double AverageReproductionThresholdGene { get; init; }
}