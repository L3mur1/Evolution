using System;

namespace Evolution.Core;

public sealed class BiomeRegionConfig
{
    public BiomeConfig Biome { get; init; } = new();

    public int CenterX { get; init; }

    public int CenterY { get; init; }

    /// <summary>
    /// Half of the side length of the square region (in tiles).
    /// </summary>
    public int HalfSize { get; init; }
}

public sealed class BiomeMapConfig
{
    public BiomeConfig DefaultBiome { get; init; } = new();

    public BiomeRegionConfig[] Regions { get; init; } = [];
}