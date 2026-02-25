namespace Evolution.Core;

public sealed class WorldState
{
    public int Version { get; init; }
    public int TickNumber { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int NextOrganismId { get; init; }

    public ulong RngState0 { get; init; }
    public ulong RngState1 { get; init; }

    public double[] Food { get; init; } = Array.Empty<double>();

    public List<OrganismState> Organisms { get; init; } = new();
}

public sealed class OrganismState
{
    public int Id { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public int HomeX { get; init; }
    public int HomeY { get; init; }
    public int TicksFarFromHome { get; init; }
    public double Energy { get; init; }
    public int Age { get; init; }
    public GenomeState Genome { get; init; } = null!;
}

public sealed class GenomeState
{
    public double FoodGainGene { get; init; }
    public double EyesGene { get; init; }
    public double ReproductionThresholdGene { get; init; }
    public double SpeedGene { get; init; }
    public double HomeRelocationGene { get; init; }
    public double WanderGene { get; init; }
}

