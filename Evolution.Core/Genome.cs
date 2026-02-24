namespace Evolution.Core;

public sealed class Genome
{
    public double FoodGainGene { get; set; }

    // 0 means "baseline"; values typically in [-1, 1].
    public double MetabolismGene { get; set; }

    public double EyesGene { get; set; }
    public double MovementBiasGene { get; set; }
    public double ReproductionThresholdGene { get; set; }
    public double SpeedGene { get; set; }
}