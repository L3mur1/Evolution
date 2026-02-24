using System.Text.Json;
using Evolution.Core;

namespace Evolution.Trainer;

public static class WorldConfigLoader
{
    private const string DefaultFileName = "worldconfig.json";

    public static WorldConfig Load(string? fileName = null)
    {
        var name = string.IsNullOrWhiteSpace(fileName) ? DefaultFileName : fileName;
        var configPath = Path.Combine(AppContext.BaseDirectory, name);

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException(
                $"Configuration file '{name}' was not found at '{configPath}'.",
                configPath);
        }

        try
        {
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<WorldConfig>(json);

            if (config is null)
            {
                throw new InvalidOperationException(
                    $"Configuration file '{name}' could not be deserialized into a WorldConfig instance.");
            }

            return config;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Configuration file '{name}' contains invalid JSON.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException(
                $"Configuration file '{name}' could not be read.", ex);
        }
    }
}

