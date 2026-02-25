namespace Evolution.Core;

public sealed class Genome
{
    public double FoodGainGene { get; set; }
    public double EyesGene { get; set; }
    public double ReproductionThresholdGene { get; set; }
    public double SpeedGene { get; set; }

    /// <summary>
    /// Scales how quickly the organism's home position relocates toward its current
    /// position relative to the configured HomeRelocationFactor.
    /// </summary>
    public double HomeRelocationGene { get; set; }

    /// <summary>
    /// Scales the energy cost of being far from home; positive values reduce the
    /// penalty (more nomadic), negative values increase it (more home-bound).
    /// </summary>
    public double WanderGene { get; set; }
}