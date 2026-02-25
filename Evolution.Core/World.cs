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
        double totalFoodGainGene = 0.0;
        double totalReproductionThresholdGene = 0.0;
        double totalEyesGene = 0.0;
        double totalSpeedGene = 0.0;

        double totalFoodGainGeneSq = 0.0;
        double totalReproductionThresholdGeneSq = 0.0;
        double totalEyesGeneSq = 0.0;
        double totalSpeedGeneSq = 0.0;

        double minEnergy = double.MaxValue;
        double maxEnergy = double.MinValue;
        int minAge = int.MaxValue;
        int maxAge = int.MinValue;

        Organism? oldest = null;
        Organism? bestEyes = null;
        Organism? worstEyes = null;
        Organism? fastest = null;
        Organism? slowest = null;
        Organism? minEnergyOrg = null;
        Organism? maxEnergyOrg = null;

        foreach (var o in organisms)
        {
            totalEnergy += o.Energy;
            totalAge += o.Age;

            var g = o.Genome;

            totalFoodGainGene += g.FoodGainGene;
            totalReproductionThresholdGene += g.ReproductionThresholdGene;
            totalEyesGene += g.EyesGene;
            totalSpeedGene += g.SpeedGene;

            totalFoodGainGeneSq += g.FoodGainGene * g.FoodGainGene;
            totalReproductionThresholdGeneSq += g.ReproductionThresholdGene * g.ReproductionThresholdGene;
            totalEyesGeneSq += g.EyesGene * g.EyesGene;
            totalSpeedGeneSq += g.SpeedGene * g.SpeedGene;

            if (o.Energy < minEnergy) minEnergy = o.Energy;
            if (o.Energy > maxEnergy) maxEnergy = o.Energy;
            if (o.Age < minAge) minAge = o.Age;
            if (o.Age > maxAge) maxAge = o.Age;

            if (oldest is null || o.Age > oldest.Age) oldest = o;
            if (bestEyes is null || g.EyesGene > bestEyes.Genome.EyesGene) bestEyes = o;
            if (worstEyes is null || g.EyesGene < worstEyes.Genome.EyesGene) worstEyes = o;
            if (fastest is null || g.SpeedGene > fastest.Genome.SpeedGene) fastest = o;
            if (slowest is null || g.SpeedGene < slowest.Genome.SpeedGene) slowest = o;
            if (minEnergyOrg is null || o.Energy < minEnergyOrg.Energy) minEnergyOrg = o;
            if (maxEnergyOrg is null || o.Energy > maxEnergyOrg.Energy) maxEnergyOrg = o;
        }

        oldest ??= organisms[0];
        bestEyes ??= organisms[0];
        worstEyes ??= organisms[0];
        fastest ??= organisms[0];
        slowest ??= organisms[0];
        minEnergyOrg ??= organisms[0];
        maxEnergyOrg ??= organisms[0];

        var pop = (double)population;

        var avgFood = totalFoodGainGene / pop;
        var avgRepro = totalReproductionThresholdGene / pop;
        var avgEyes = totalEyesGene / pop;
        var avgSpeed = totalSpeedGene / pop;

        static double Variance(double sum, double sumSq, double n) =>
            sumSq / n - (sum / n) * (sum / n);

        var foodStd = Math.Sqrt(Math.Max(0.0, Variance(totalFoodGainGene, totalFoodGainGeneSq, pop)));
        var reproStd = Math.Sqrt(Math.Max(0.0, Variance(totalReproductionThresholdGene, totalReproductionThresholdGeneSq, pop)));
        var eyesStd = Math.Sqrt(Math.Max(0.0, Variance(totalEyesGene, totalEyesGeneSq, pop)));
        var speedStd = Math.Sqrt(Math.Max(0.0, Variance(totalSpeedGene, totalSpeedGeneSq, pop)));

        var avgEnergy = totalEnergy / pop;
        var typical = organisms
            .OrderBy(o => Math.Abs(o.Energy - avgEnergy))
            .First();

        return new WorldStats
        {
            Tick = TickNumber,
            Population = population,
            AverageEnergy = avgEnergy,
            AverageAge = totalAge / pop,
            AverageFoodGainGene = avgFood,
            AverageReproductionThresholdGene = avgRepro,
            AverageEyesGene = avgEyes,
            AverageSpeedGene = avgSpeed,
            FoodGainGeneStdDev = foodStd,
            ReproductionThresholdGeneStdDev = reproStd,
            EyesGeneStdDev = eyesStd,
            SpeedGeneStdDev = speedStd,
            MinEnergy = minEnergy,
            MaxEnergy = maxEnergy,
            MinAge = minAge,
            MaxAge = maxAge,

            OldestAge = oldest.Age,
            OldestEnergy = oldest.Energy,
            OldestFoodGainGene = oldest.Genome.FoodGainGene,
            OldestReproductionThresholdGene = oldest.Genome.ReproductionThresholdGene,
            OldestEyesGene = oldest.Genome.EyesGene,
            OldestSpeedGene = oldest.Genome.SpeedGene,

            BestEyesAge = bestEyes.Age,
            BestEyesEnergy = bestEyes.Energy,
            BestEyesFoodGainGene = bestEyes.Genome.FoodGainGene,
            BestEyesReproductionThresholdGene = bestEyes.Genome.ReproductionThresholdGene,
            BestEyesEyesGene = bestEyes.Genome.EyesGene,
            BestEyesSpeedGene = bestEyes.Genome.SpeedGene,

            WorstEyesAge = worstEyes.Age,
            WorstEyesEnergy = worstEyes.Energy,
            WorstEyesFoodGainGene = worstEyes.Genome.FoodGainGene,
            WorstEyesReproductionThresholdGene = worstEyes.Genome.ReproductionThresholdGene,
            WorstEyesEyesGene = worstEyes.Genome.EyesGene,
            WorstEyesSpeedGene = worstEyes.Genome.SpeedGene,

            FastestAge = fastest.Age,
            FastestEnergy = fastest.Energy,
            FastestFoodGainGene = fastest.Genome.FoodGainGene,
            FastestReproductionThresholdGene = fastest.Genome.ReproductionThresholdGene,
            FastestEyesGene = fastest.Genome.EyesGene,
            FastestSpeedGene = fastest.Genome.SpeedGene,

            SlowestAge = slowest.Age,
            SlowestEnergy = slowest.Energy,
            SlowestFoodGainGene = slowest.Genome.FoodGainGene,
            SlowestReproductionThresholdGene = slowest.Genome.ReproductionThresholdGene,
            SlowestEyesGene = slowest.Genome.EyesGene,
            SlowestSpeedGene = slowest.Genome.SpeedGene,

            MinEnergyAge = minEnergyOrg.Age,
            MinEnergyEnergy = minEnergyOrg.Energy,
            MinEnergyFoodGainGene = minEnergyOrg.Genome.FoodGainGene,
            MinEnergyReproductionThresholdGene = minEnergyOrg.Genome.ReproductionThresholdGene,
            MinEnergyEyesGene = minEnergyOrg.Genome.EyesGene,
            MinEnergySpeedGene = minEnergyOrg.Genome.SpeedGene,

            MaxEnergyAge = maxEnergyOrg.Age,
            MaxEnergyEnergy = maxEnergyOrg.Energy,
            MaxEnergyFoodGainGene = maxEnergyOrg.Genome.FoodGainGene,
            MaxEnergyReproductionThresholdGene = maxEnergyOrg.Genome.ReproductionThresholdGene,
            MaxEnergyEyesGene = maxEnergyOrg.Genome.EyesGene,
            MaxEnergySpeedGene = maxEnergyOrg.Genome.SpeedGene,

            TypicalAge = typical.Age,
            TypicalEnergy = typical.Energy,
            TypicalFoodGainGene = typical.Genome.FoodGainGene,
            TypicalReproductionThresholdGene = typical.Genome.ReproductionThresholdGene,
            TypicalEyesGene = typical.Genome.EyesGene,
            TypicalSpeedGene = typical.Genome.SpeedGene
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

            // Derive per-organism traits from genome (constant base metabolism plus gene-driven metabolic load, gene-driven food gain and reproduction).
            var baseMetabolism = config.MetabolismBase;
            var extraMetabolicLoad = ComputeExtraMetabolicLoad(genome);

            var foodFactor = 1.0 + config.FoodGainGeneScale * genome.FoodGainGene;
            if (foodFactor < 0.0) foodFactor = 0.0;
            var energyFromFood = config.FoodGainBase * foodFactor;

            var reproductionThreshold = config.ReproductionThresholdBase *
                                        (1.0 + config.ReproductionThresholdGeneScale * genome.ReproductionThresholdGene);

            var senseRadius = ComputeSenseRadius(genome);

            var energyLossPerTick = baseMetabolism + extraMetabolicLoad;

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

            // Movement: no sensing when senseRadius <= 0 (random only); otherwise food-aware within senseRadius
            int chosenDir;
            if (senseRadius <= 0.0)
            {
                chosenDir = rng.Next(4);
            }
            else
            {
                var r = Math.Max(1, (int)Math.Round(senseRadius));
                var scores = new[]
                {
                    ScoreDirection(organism.X, organism.Y, 0, r),
                    ScoreDirection(organism.X, organism.Y, 1, r),
                    ScoreDirection(organism.X, organism.Y, 2, r),
                    ScoreDirection(organism.X, organism.Y, 3, r)
                };
                if (scores.All(s => s <= 0.0))
                {
                    chosenDir = rng.Next(4);
                }
                else if (rng.NextDouble() < 0.7)
                {
                    var max = scores.Max();
                    var bestDirs = Enumerable.Range(0, 4).Where(i => scores[i] == max).ToArray();
                    chosenDir = bestDirs[rng.Next(bestDirs.Length)];
                }
                else
                {
                    chosenDir = rng.Next(4);
                }
            }

            // Speed: chance of 2 steps; gene can increase or decrease this chance
            var speedFactor = genome.SpeedGene;
            var pTwo = Math.Clamp(
                config.SpeedBaseTwoStepChance + speedFactor * config.SpeedChanceScale,
                0.0, 1.0);
            var extraCost = config.SpeedExtraStepCost;
            var steps = (rng.NextDouble() < pTwo && organism.Energy >= extraCost) ? 2 : 1;
            var (mx, my) = WrappedStep(organism.X, organism.Y, chosenDir, steps);
            organism.X = mx;
            organism.Y = my;
            organism.Energy -= (steps - 1) * extraCost;

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
            FoodGainGene = 0.0,
            ReproductionThresholdGene = 0.0,
            EyesGene = 0.0,
            SpeedGene = 0.0
        };

    private Genome CloneAndMutateGenome(Genome parent)
    {
        var child = new Genome
        {
            FoodGainGene = MutateGene(parent.FoodGainGene),
            ReproductionThresholdGene = MutateGene(parent.ReproductionThresholdGene),
            EyesGene = MutateGene(parent.EyesGene),
            SpeedGene = MutateGene(parent.SpeedGene)
        };

        return child;
    }

    private double ComputeExtraMetabolicLoad(Genome genome)
    {
        var extraMetabolicLoad = 0.0;

        var eyesPositive = Math.Max(0.0, genome.EyesGene);
        var eyesNegative = Math.Min(0.0, genome.EyesGene);
        extraMetabolicLoad += config.EyesEnergyCostPositive * eyesPositive * eyesPositive;
        extraMetabolicLoad += config.EyesEnergyCostNegative * eyesNegative * eyesNegative;

        var foodPositive = Math.Max(0.0, genome.FoodGainGene);
        var foodNegative = Math.Min(0.0, genome.FoodGainGene);
        extraMetabolicLoad += config.FoodGainPenaltyPositive * foodPositive * foodPositive;
        extraMetabolicLoad += config.FoodGainPenaltyNegative * foodNegative * foodNegative;

        var speedPositive = Math.Max(0.0, genome.SpeedGene);
        var speedNegative = Math.Min(0.0, genome.SpeedGene);
        extraMetabolicLoad += config.SpeedGeneCostPositive * speedPositive * speedPositive;
        extraMetabolicLoad += config.SpeedGeneCostNegative * speedNegative * speedNegative;

        return extraMetabolicLoad;
    }

    private double ComputeSenseRadius(Genome genome)
    {
        var radius = config.EyesBaseRadius * (1.0 + config.EyesRadiusScale * genome.EyesGene);
        if (radius < 0.0)
        {
            radius = 0.0;
        }
        return radius;
    }

    private void MoveRandomly(Organism organism)
    {
        var direction = rng.Next(4);
        var (x, y) = WrappedStep(organism.X, organism.Y, direction, 1);
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

    private double ScoreDirection(int x, int y, int dir, int radius)
    {
        double score = 0.0;
        for (var step = 1; step <= radius; step++)
        {
            var (nx, ny) = WrappedStep(x, y, dir, step);
            score += food[nx, ny];
        }
        return score;
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

    private (int x, int y) WrappedStep(int x, int y, int dir, int steps)
    {
        switch (dir)
        {
            case 0: y -= steps; break; // North
            case 1: y += steps; break; // South
            case 2: x -= steps; break; // West
            case 3: x += steps; break; // East
        }

        if (x < 0) x += config.Width;
        if (x >= config.Width) x -= config.Width;
        if (y < 0) y += config.Height;
        if (y >= config.Height) y -= config.Height;
        return (x, y);
    }
}