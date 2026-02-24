namespace Evolution.Core;

public sealed class Organism
{
    public int Age { get; set; }
    public double Energy { get; set; }
    public int Id { get; init; }
    public int X { get; set; }
    public int Y { get; set; }
}