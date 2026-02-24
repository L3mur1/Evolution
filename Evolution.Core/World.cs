using System.Collections.ObjectModel;

namespace Evolution.Core;

public sealed class World
{
    private readonly WorldConfig config;
    private readonly double[,] food;
    private readonly List<Organism> organisms = [];
    private readonly Random rng;
    private int nextOrganismId = 1;

    public ReadOnlyCollection<Organism> Organisms => organisms.AsReadOnly();
    public int TickNumber { get; private set; }

    public World(WorldConfig config)
    {
        this.config = config;
        rng = new Random(config.RandomSeed);
        food = new double[config.Width, config.Height];

        SeedFood();
        SeedOrganisms();
    }

    public WorldStats GetStats()
    {
        var population = organisms.Count;

        if (population == 0)
        {
            return new WorldStats
            {
                Tick = TickNumber,
                Population = 0,
                AverageEnergy = 0.0,
                AverageAge = 0.0
            };
        }

        double totalEnergy = 0.0;
        double totalAge = 0.0;

        foreach (var o in organisms)
        {
            totalEnergy += o.Energy;
            totalAge += o.Age;
        }

        return new WorldStats
        {
            Tick = TickNumber,
            Population = population,
            AverageEnergy = totalEnergy / population,
            AverageAge = totalAge / population
        };
    }

    public void Tick()
    {
        TickNumber++;

        if (organisms.Count == 0)
        {
            RegenerateFood();
            return;
        }

        var dead = new List<Organism>(organisms.Count);
        var newborns = new List<Organism>();

        // Work on a snapshot to avoid modifying the list while iterating.
        var snapshot = organisms.ToArray();

        foreach (var organism in snapshot)
        {
            // Energy loss
            organism.Energy -= config.EnergyLossPerTick;
            if (organism.Energy <= 0)
            {
                dead.Add(organism);
                continue;
            }

            // Consume food at current cell
            if (food[organism.X, organism.Y] > 0.0)
            {
                food[organism.X, organism.Y] -= 1.0;
                organism.Energy += config.EnergyFromFood;
            }

            // Move randomly N/S/E/W with wrap-around
            MoveRandomly(organism);

            // Age
            organism.Age++;

            // Reproduction
            if (organism.Energy >= config.ReproductionThreshold)
            {
                organism.Energy -= config.ReproductionCost;

                var child = new Organism
                {
                    Id = nextOrganismId++,
                    X = organism.X,
                    Y = organism.Y,
                    Energy = config.StartEnergy,
                    Age = 0
                };

                newborns.Add(child);
            }
        }

        // Apply deaths and births
        if (dead.Count > 0)
        {
            foreach (var d in dead)
            {
                organisms.Remove(d);
            }
        }

        if (newborns.Count > 0)
        {
            organisms.AddRange(newborns);
        }

        // Food regeneration after organisms act
        RegenerateFood();
    }

    private void MoveRandomly(Organism organism)
    {
        var direction = rng.Next(4);

        var x = organism.X;
        var y = organism.Y;

        switch (direction)
        {
            case 0: // North
                y -= 1;
                break;

            case 1: // South
                y += 1;
                break;

            case 2: // West
                x -= 1;
                break;

            case 3: // East
                x += 1;
                break;
        }

        // Wrap-around world
        if (x < 0) x = config.Width - 1;
        if (x >= config.Width) x = 0;
        if (y < 0) y = config.Height - 1;
        if (y >= config.Height) y = 0;

        organism.X = x;
        organism.Y = y;
    }

    private void RegenerateFood()
    {
        for (var x = 0; x < config.Width; x++)
        {
            for (var y = 0; y < config.Height; y++)
            {
                if (rng.NextDouble() < config.FoodRegenProbability)
                {
                    food[x, y] += 1.0;
                }
            }
        }
    }

    private void SeedFood()
    {
        for (var x = 0; x < config.Width; x++)
        {
            for (var y = 0; y < config.Height; y++)
            {
                if (rng.NextDouble() < config.InitialFoodDensity)
                {
                    food[x, y] += 1.0;
                }
            }
        }
    }

    private void SeedOrganisms()
    {
        for (var i = 0; i < config.InitialOrganismCount; i++)
        {
            var x = rng.Next(config.Width);
            var y = rng.Next(config.Height);

            organisms.Add(new Organism
            {
                Id = nextOrganismId++,
                X = x,
                Y = y,
                Energy = config.StartEnergy,
                Age = 0
            });
        }
    }
}