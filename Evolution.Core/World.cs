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
        double totalMetabolismGene = 0.0;
        double totalFoodGainGene = 0.0;
        double totalReproductionThresholdGene = 0.0;

        double minEnergy = double.MaxValue;
        double maxEnergy = double.MinValue;
        int minAge = int.MaxValue;
        int maxAge = int.MinValue;

        int metabLow = 0, metabMid = 0, metabHigh = 0;
        int foodLow = 0, foodMid = 0, foodHigh = 0;
        int reproLow = 0, reproMid = 0, reproHigh = 0;

        Organism? oldest = null;

        foreach (var o in organisms)
        {
            totalEnergy += o.Energy;
            totalAge += o.Age;
            totalMetabolismGene += o.Genome.MetabolismGene;
            totalFoodGainGene += o.Genome.FoodGainGene;
            totalReproductionThresholdGene += o.Genome.ReproductionThresholdGene;

            if (o.Energy < minEnergy) minEnergy = o.Energy;
            if (o.Energy > maxEnergy) maxEnergy = o.Energy;
            if (o.Age < minAge) minAge = o.Age;
            if (o.Age > maxAge) maxAge = o.Age;

            BucketGene(o.Genome.MetabolismGene, ref metabLow, ref metabMid, ref metabHigh);
            BucketGene(o.Genome.FoodGainGene, ref foodLow, ref foodMid, ref foodHigh);
            BucketGene(o.Genome.ReproductionThresholdGene, ref reproLow, ref reproMid, ref reproHigh);

            if (oldest is null || o.Age > oldest.Age)
            {
                oldest = o;
            }
        }

        oldest ??= organisms[0];

        return new WorldStats
        {
            Tick = TickNumber,
            Population = population,
            AverageEnergy = totalEnergy / population,
            AverageAge = totalAge / population,
            AverageMetabolismGene = totalMetabolismGene / population,
            AverageFoodGainGene = totalFoodGainGene / population,
            AverageReproductionThresholdGene = totalReproductionThresholdGene / population,
            MinEnergy = minEnergy,
            MaxEnergy = maxEnergy,
            MinAge = minAge,
            MaxAge = maxAge,
            MetabolismLowCount = metabLow,
            MetabolismMidCount = metabMid,
            MetabolismHighCount = metabHigh,
            FoodGainLowCount = foodLow,
            FoodGainMidCount = foodMid,
            FoodGainHighCount = foodHigh,
            ReproductionThresholdLowCount = reproLow,
            ReproductionThresholdMidCount = reproMid,
            ReproductionThresholdHighCount = reproHigh,
            SampleEnergy = oldest.Energy,
            SampleAge = oldest.Age,
            SampleMetabolismGene = oldest.Genome.MetabolismGene,
            SampleFoodGainGene = oldest.Genome.FoodGainGene,
            SampleReproductionThresholdGene = oldest.Genome.ReproductionThresholdGene
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
            var genome = organism.Genome;

            // Derive per-organism traits from genome.
            var energyLossPerTick = config.MetabolismBase * (1.0 + config.MetabolismGeneScale * genome.MetabolismGene);
            var energyFromFood = config.FoodGainBase * (1.0 + config.FoodGainGeneScale * genome.FoodGainGene);
            var reproductionThreshold = config.ReproductionThresholdBase *
                                        (1.0 + config.ReproductionThresholdGeneScale * genome.ReproductionThresholdGene);

            // Energy loss
            organism.Energy -= energyLossPerTick;
            if (organism.Energy <= 0)
            {
                dead.Add(organism);
                continue;
            }

            // Consume food at current cell
            if (food[organism.X, organism.Y] > 0.0)
            {
                food[organism.X, organism.Y] -= 1.0;
                organism.Energy += energyFromFood;
            }

            // Move randomly N/S/E/W with wrap-around
            MoveRandomly(organism);

            // Age
            organism.Age++;

            // Death rule: old + low-energy organisms have
            // an extra chance of dying each tick.
            if (organism.Age > config.OldAgeThreshold &&
                organism.Energy < config.OldAgeEnergyThreshold &&
                rng.NextDouble() < config.OldAgeLowEnergyDeathProbability)
            {
                dead.Add(organism);
                continue;
            }

            // Reproduction
            if (organism.Energy >= reproductionThreshold)
            {
                organism.Energy -= config.ReproductionCost;

                var child = new Organism
                {
                    Id = nextOrganismId++,
                    X = organism.X,
                    Y = organism.Y,
                    Energy = config.StartEnergy,
                    Age = 0,
                    Genome = CloneAndMutateGenome(genome)
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

    private static Genome CreateBaselineGenome() =>
        new()
        {
            MetabolismGene = 0.0,
            FoodGainGene = 0.0,
            ReproductionThresholdGene = 0.0,
            MovementBiasGene = 0.0
        };

    private Genome CloneAndMutateGenome(Genome parent)
    {
        var child = new Genome
        {
            MetabolismGene = MutateGene(parent.MetabolismGene),
            FoodGainGene = MutateGene(parent.FoodGainGene),
            ReproductionThresholdGene = MutateGene(parent.ReproductionThresholdGene),
            MovementBiasGene = MutateGene(parent.MovementBiasGene)
        };

        return child;
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

    private double MutateGene(double gene)
    {
        if (rng.NextDouble() < config.MutationProbabilityPerGene)
        {
            var step = (rng.NextDouble() * 2.0 - 1.0) * config.MutationStepSize;
            gene += step;

            // Keep genes within a reasonable range so trait effects stay bounded.
            if (gene < -1.0) gene = -1.0;
            if (gene > 1.0) gene = 1.0;
        }

        return gene;
    }

    private static void BucketGene(double value, ref int low, ref int mid, ref int high)
    {
        if (value < -0.5)
        {
            low++;
        }
        else if (value > 0.5)
        {
            high++;
        }
        else
        {
            mid++;
        }
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
                Age = 0,
                Genome = CreateBaselineGenome()
            });
        }
    }
}