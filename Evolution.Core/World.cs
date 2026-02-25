using System.Collections.ObjectModel;
using System.Linq;

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

        double minEnergy = double.MaxValue;
        double maxEnergy = double.MinValue;
        int minAge = int.MaxValue;
        int maxAge = int.MinValue;

        int foodLow = 0, foodMid = 0, foodHigh = 0;
        int reproLow = 0, reproMid = 0, reproHigh = 0;
        int eyesLow = 0, eyesMid = 0, eyesHigh = 0;
        int speedLow = 0, speedMid = 0, speedHigh = 0;

        Organism? oldest = null;

        foreach (var o in organisms)
        {
            totalEnergy += o.Energy;
            totalAge += o.Age;
            totalFoodGainGene += o.Genome.FoodGainGene;
            totalReproductionThresholdGene += o.Genome.ReproductionThresholdGene;
            totalEyesGene += o.Genome.EyesGene;
            totalSpeedGene += o.Genome.SpeedGene;

            if (o.Energy < minEnergy) minEnergy = o.Energy;
            if (o.Energy > maxEnergy) maxEnergy = o.Energy;
            if (o.Age < minAge) minAge = o.Age;
            if (o.Age > maxAge) maxAge = o.Age;

            BucketGene(o.Genome.FoodGainGene, ref foodLow, ref foodMid, ref foodHigh);
            BucketGene(o.Genome.ReproductionThresholdGene, ref reproLow, ref reproMid, ref reproHigh);
            BucketGene(o.Genome.EyesGene, ref eyesLow, ref eyesMid, ref eyesHigh);
            BucketGene(o.Genome.SpeedGene, ref speedLow, ref speedMid, ref speedHigh);

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
            AverageFoodGainGene = totalFoodGainGene / population,
            AverageReproductionThresholdGene = totalReproductionThresholdGene / population,
            AverageEyesGene = totalEyesGene / population,
            AverageSpeedGene = totalSpeedGene / population,
            MinEnergy = minEnergy,
            MaxEnergy = maxEnergy,
            MinAge = minAge,
            MaxAge = maxAge,
            FoodGainLowCount = foodLow,
            FoodGainMidCount = foodMid,
            FoodGainHighCount = foodHigh,
            ReproductionThresholdLowCount = reproLow,
            ReproductionThresholdMidCount = reproMid,
            ReproductionThresholdHighCount = reproHigh,
            EyesLowCount = eyesLow,
            EyesMidCount = eyesMid,
            EyesHighCount = eyesHigh,
            SpeedLowCount = speedLow,
            SpeedMidCount = speedMid,
            SpeedHighCount = speedHigh,
            SampleEnergy = oldest.Energy,
            SampleAge = oldest.Age,
            SampleFoodGainGene = oldest.Genome.FoodGainGene,
            SampleReproductionThresholdGene = oldest.Genome.ReproductionThresholdGene,
            SampleEyesGene = oldest.Genome.EyesGene,
            SampleSpeedGene = oldest.Genome.SpeedGene
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

            // Derive per-organism traits from genome (constant metabolism, gene-driven food gain and reproduction).
            var energyLossPerTick = config.MetabolismBase;
            var energyFromFood = config.FoodGainBase * (1.0 + config.FoodGainGeneScale * genome.FoodGainGene);
            var reproductionThreshold = config.ReproductionThresholdBase *
                                        (1.0 + config.ReproductionThresholdGeneScale * genome.ReproductionThresholdGene);

            // EyesGene <= 0: no sensing, no eyes cost. EyesGene > 0: sense radius and quadratic cost.
            var eyesExtra = Math.Max(0.0, genome.EyesGene);
            var senseRadius = config.EyesBaseRadius + config.EyesRadiusScale * eyesExtra;
            var eyesCost = genome.EyesGene > 0
                ? config.EyesEnergyCostCoefficient * senseRadius * senseRadius
                : 0.0;
            energyLossPerTick += eyesCost;

            var foodGenePositive = Math.Max(0.0, genome.FoodGainGene);
            var extraFoodGeneCost = config.FoodGainPenaltyCoefficient * foodGenePositive * foodGenePositive;
            energyLossPerTick += extraFoodGeneCost;

            // Option A: per-tick cost for having higher SpeedGene (like eyes metabolism penalty)
            var speedGeneCost = genome.SpeedGene > 0
                ? config.SpeedGeneCostCoefficient * genome.SpeedGene
                : 0.0;
            energyLossPerTick += speedGeneCost;

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

            // Movement: no sensing when EyesGene <= 0 (random only); otherwise food-aware within senseRadius
            int chosenDir;
            if (genome.EyesGene <= 0.0)
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

            // Speed: chance of 2 steps (factor); pay extra only when taking 2 steps; fall back to 1 if can't afford
            var pTwo = Math.Min(1.0, Math.Max(0.0, genome.SpeedGene) * config.SpeedChanceScale);
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
            MovementBiasGene = 0.0,
            EyesGene = 0.0,
            SpeedGene = 0.0
        };

    private Genome CloneAndMutateGenome(Genome parent)
    {
        var child = new Genome
        {
            FoodGainGene = MutateGene(parent.FoodGainGene),
            ReproductionThresholdGene = MutateGene(parent.ReproductionThresholdGene),
            MovementBiasGene = MutateGene(parent.MovementBiasGene),
            EyesGene = MutateGene(parent.EyesGene),
            SpeedGene = MutateGene(parent.SpeedGene)
        };

        return child;
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