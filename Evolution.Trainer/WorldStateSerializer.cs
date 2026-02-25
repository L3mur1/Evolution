using System.Text.Json;
using Evolution.Core;

namespace Evolution.Trainer;

public static class WorldStateSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string Serialize(WorldState state) =>
        JsonSerializer.Serialize(state, Options);

    public static WorldState? Deserialize(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<WorldState>(json, Options);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static World RestoreWorld(WorldConfig config, WorldState state) =>
        World.FromState(config, state);
}

