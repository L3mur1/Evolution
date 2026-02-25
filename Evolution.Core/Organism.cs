namespace Evolution.Core;

public sealed class Organism
{
    public Genome Genome { get; init; } = null!;
    public int Age { get; set; }
    public double Energy { get; set; }
    public int Id { get; init; }
    public int X { get; set; }
    public int Y { get; set; }

    /// <summary>
    /// Center of this organism's typical home range (habitat).
    /// </summary>
    public int HomeX { get; set; }

    public int HomeY { get; set; }

    /// <summary>
    /// Number of consecutive ticks spent beyond the configured home radius.
    /// Used to drive slow habitat migration.
    /// </summary>
    public int TicksFarFromHome { get; set; }
}