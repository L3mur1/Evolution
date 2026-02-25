namespace Evolution.Core;

/// <summary>
/// Simple deterministic PRNG with explicit, serializable state.
/// Based on a 128-bit xorshift variant.
/// </summary>
public sealed class DeterministicRandom
{
    private ulong state0;
    private ulong state1;

    public ulong State0
    {
        get => state0;
        set => state0 = value == 0 ? 1UL : value;
    }

    public ulong State1
    {
        get => state1;
        set => state1 = value == 0 ? 2UL : value;
    }

    public DeterministicRandom(ulong seed)
    {
        // SplitMix64 to expand a single seed into two non-zero states.
        ulong z = seed + 0x9E3779B97F4A7C15UL;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        var s0 = z ^ (z >> 31);

        z += 0x9E3779B97F4A7C15UL;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        var s1 = z ^ (z >> 31);

        if (s0 == 0) s0 = 1;
        if (s1 == 0) s1 = 2;

        state0 = s0;
        state1 = s1;
    }

    private DeterministicRandom(ulong state0, ulong state1)
    {
        State0 = state0;
        State1 = state1;
    }

    public static DeterministicRandom FromState(ulong state0, ulong state1) =>
        new(state0, state1);

    private ulong NextUInt64()
    {
        var s1 = state0;
        var s0 = state1;
        state0 = s0;
        s1 ^= s1 << 23;
        state1 = s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26);
        return state1 + s0;
    }

    public double NextDouble()
    {
        // Use 53 significant bits to generate a double in [0,1).
        return (NextUInt64() >> 11) * (1.0 / (1UL << 53));
    }

    public int Next(int maxValue)
    {
        if (maxValue <= 0) throw new ArgumentOutOfRangeException(nameof(maxValue));
        return (int)(NextUInt64() % (uint)maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        if (minValue >= maxValue) throw new ArgumentOutOfRangeException(nameof(maxValue));
        var range = (uint)(maxValue - minValue);
        return minValue + (int)(NextUInt64() % range);
    }
}

