using System.Collections.ObjectModel;

namespace Evolution.Core;

public sealed class World
{
    private readonly WorldConfig config;
    private readonly double[,] food;
    private readonly BiomeConfig[,] biomeAtCell;
    private readonly List<Organism> organisms = [];
    private readonly Random rng;
    private int nextOrganismId = 1;

    private readonly Dictionary<BiomeConfig, int> biomeCellCounts = [];
    private readonly Dictionary<BiomeConfig, int> biomeBirthsLastTick = [];
    private readonly Dictionary<BiomeConfig, int> biomeDeathsLastTick = [];

    public ReadOnlyCollection<Organism> Organisms => organisms.AsReadOnly();
    public int TickNumber { get; private set; }

    public World(WorldConfig config)
    {
        this.config = config;
        rng = new Random(config.RandomSeed);
        food = new double[config.Width, config.Height];

        biomeAtCell = new BiomeConfig[config.Width, config.Height];
        InitializeBiomeMap();

        SeedFood();
        SeedOrganisms();
    }

    private void InitializeBiomeMap()
    {
        var biomeMap = config.BiomeMap ?? new BiomeMapConfig
        {
            DefaultBiome = config.Biome,
            Regions = []
        };

        var width = config.Width;
        var height = config.Height;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                biomeAtCell[x, y] = biomeMap.DefaultBiome;
            }
        }

        foreach (var region in biomeMap.Regions ?? [])
        {
            var biome = region.Biome;
            var priority = biome.Priority;

            var xMin = Math.Max(0, region.CenterX - region.HalfSize);
            var xMax = Math.Min(width - 1, region.CenterX + region.HalfSize);
            var yMin = Math.Max(0, region.CenterY - region.HalfSize);
            var yMax = Math.Min(height - 1, region.CenterY + region.HalfSize);

            for (var x = xMin; x <= xMax; x++)
            {
                for (var y = yMin; y <= yMax; y++)
                {
                    if (priority > biomeAtCell[x, y].Priority)
                    {
                        biomeAtCell[x, y] = biome;
                    }
                }
            }
        }

        biomeCellCounts.Clear();
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var biome = biomeAtCell[x, y];
                if (!biomeCellCounts.TryGetValue(biome, out var count))
                {
                    count = 0;
                }
                biomeCellCounts[biome] = count + 1;
            }
        }
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
        double totalHomeRelocationGene = 0.0;
        double totalWanderGene = 0.0;

        double totalFoodGainGeneSq = 0.0;
        double totalReproductionThresholdGeneSq = 0.0;
        double totalEyesGeneSq = 0.0;
        double totalSpeedGeneSq = 0.0;
        double totalHomeRelocationGeneSq = 0.0;
        double totalWanderGeneSq = 0.0;

        var biomeAccumulators = new Dictionary<BiomeConfig, BiomeAccumulator>();

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

            var biome = biomeAtCell[o.X, o.Y];
            if (!biomeAccumulators.TryGetValue(biome, out var biomeAcc))
            {
                biomeAcc = new BiomeAccumulator();
                biomeAccumulators[biome] = biomeAcc;
            }

            biomeAcc.Population++;
            biomeAcc.TotalEnergy += o.Energy;
            biomeAcc.TotalAge += o.Age;

            totalFoodGainGene += g.FoodGainGene;
            totalReproductionThresholdGene += g.ReproductionThresholdGene;
            totalEyesGene += g.EyesGene;
            totalSpeedGene += g.SpeedGene;
            totalHomeRelocationGene += g.HomeRelocationGene;
            totalWanderGene += g.WanderGene;

            biomeAcc.TotalFoodGainGene += g.FoodGainGene;
            biomeAcc.TotalReproductionThresholdGene += g.ReproductionThresholdGene;
            biomeAcc.TotalEyesGene += g.EyesGene;
            biomeAcc.TotalSpeedGene += g.SpeedGene;
            biomeAcc.TotalHomeRelocationGene += g.HomeRelocationGene;
            biomeAcc.TotalWanderGene += g.WanderGene;

            totalFoodGainGeneSq += g.FoodGainGene * g.FoodGainGene;
            totalReproductionThresholdGeneSq += g.ReproductionThresholdGene * g.ReproductionThresholdGene;
            totalEyesGeneSq += g.EyesGene * g.EyesGene;
            totalSpeedGeneSq += g.SpeedGene * g.SpeedGene;
            totalHomeRelocationGeneSq += g.HomeRelocationGene * g.HomeRelocationGene;
            totalWanderGeneSq += g.WanderGene * g.WanderGene;

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
        var avgHomeRelocation = totalHomeRelocationGene / pop;
        var avgWander = totalWanderGene / pop;

        static double Variance(double sum, double sumSq, double n) =>
            sumSq / n - (sum / n) * (sum / n);

        var foodStd = Math.Sqrt(Math.Max(0.0, Variance(totalFoodGainGene, totalFoodGainGeneSq, pop)));
        var reproStd = Math.Sqrt(Math.Max(0.0, Variance(totalReproductionThresholdGene, totalReproductionThresholdGeneSq, pop)));
        var eyesStd = Math.Sqrt(Math.Max(0.0, Variance(totalEyesGene, totalEyesGeneSq, pop)));
        var speedStd = Math.Sqrt(Math.Max(0.0, Variance(totalSpeedGene, totalSpeedGeneSq, pop)));
        var homeRelocationStd = Math.Sqrt(Math.Max(0.0, Variance(totalHomeRelocationGene, totalHomeRelocationGeneSq, pop)));
        var wanderStd = Math.Sqrt(Math.Max(0.0, Variance(totalWanderGene, totalWanderGeneSq, pop)));

        var biomeFoodTotals = new Dictionary<BiomeConfig, double>();
        var width = config.Width;
        var height = config.Height;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var biome = biomeAtCell[x, y];
                if (!biomeFoodTotals.TryGetValue(biome, out var totalFood))
                {
                    totalFood = 0.0;
                }
                totalFood += food[x, y];
                biomeFoodTotals[biome] = totalFood;
            }
        }

        var biomeStatsList = new List<BiomeStats>(biomeAccumulators.Count);
        foreach (var kvp in biomeAccumulators)
        {
            var biome = kvp.Key;
            var acc = kvp.Value;

            var biomePopulation = acc.Population;
            var biomePopulationDouble = (double)biomePopulation;

            biomeCellCounts.TryGetValue(biome, out var cellCount);
            var density = cellCount > 0 ? biomePopulationDouble / cellCount : 0.0;

            biomeFoodTotals.TryGetValue(biome, out var totalFoodInBiome);
            var averageFoodPerCell = cellCount > 0 ? totalFoodInBiome / cellCount : 0.0;

            biomeBirthsLastTick.TryGetValue(biome, out var births);
            biomeDeathsLastTick.TryGetValue(biome, out var deaths);

            var averageEnergyBiome = acc.TotalEnergy / biomePopulationDouble;
            var averageAgeBiome = acc.TotalAge / biomePopulationDouble;

            var biomeStats = new BiomeStats
            {
                BiomeName = biome.Name,
                CellCount = cellCount,
                Population = biomePopulation,
                Density = density,
                AverageEnergy = averageEnergyBiome,
                AverageAge = averageAgeBiome,
                AverageFoodGainGene = acc.TotalFoodGainGene / biomePopulationDouble,
                AverageReproductionThresholdGene = acc.TotalReproductionThresholdGene / biomePopulationDouble,
                AverageEyesGene = acc.TotalEyesGene / biomePopulationDouble,
                AverageSpeedGene = acc.TotalSpeedGene / biomePopulationDouble,
                AverageHomeRelocationGene = acc.TotalHomeRelocationGene / biomePopulationDouble,
                AverageWanderGene = acc.TotalWanderGene / biomePopulationDouble,
                AverageFoodPerCell = averageFoodPerCell,
                BirthsLastTick = births,
                DeathsLastTick = deaths
            };

            biomeStatsList.Add(biomeStats);
        }

        biomeStatsList.Sort((a, b) => b.Population.CompareTo(a.Population));

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
            AverageHomeRelocationGene = avgHomeRelocation,
            AverageWanderGene = avgWander,
            FoodGainGeneStdDev = foodStd,
            ReproductionThresholdGeneStdDev = reproStd,
            EyesGeneStdDev = eyesStd,
            SpeedGeneStdDev = speedStd,
            HomeRelocationGeneStdDev = homeRelocationStd,
            WanderGeneStdDev = wanderStd,
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
            OldestHomeRelocationGene = oldest.Genome.HomeRelocationGene,
            OldestWanderGene = oldest.Genome.WanderGene,

            BestEyesAge = bestEyes.Age,
            BestEyesEnergy = bestEyes.Energy,
            BestEyesFoodGainGene = bestEyes.Genome.FoodGainGene,
            BestEyesReproductionThresholdGene = bestEyes.Genome.ReproductionThresholdGene,
            BestEyesEyesGene = bestEyes.Genome.EyesGene,
            BestEyesSpeedGene = bestEyes.Genome.SpeedGene,
            BestEyesHomeRelocationGene = bestEyes.Genome.HomeRelocationGene,
            BestEyesWanderGene = bestEyes.Genome.WanderGene,

            WorstEyesAge = worstEyes.Age,
            WorstEyesEnergy = worstEyes.Energy,
            WorstEyesFoodGainGene = worstEyes.Genome.FoodGainGene,
            WorstEyesReproductionThresholdGene = worstEyes.Genome.ReproductionThresholdGene,
            WorstEyesEyesGene = worstEyes.Genome.EyesGene,
            WorstEyesSpeedGene = worstEyes.Genome.SpeedGene,
            WorstEyesHomeRelocationGene = worstEyes.Genome.HomeRelocationGene,
            WorstEyesWanderGene = worstEyes.Genome.WanderGene,

            FastestAge = fastest.Age,
            FastestEnergy = fastest.Energy,
            FastestFoodGainGene = fastest.Genome.FoodGainGene,
            FastestReproductionThresholdGene = fastest.Genome.ReproductionThresholdGene,
            FastestEyesGene = fastest.Genome.EyesGene,
            FastestSpeedGene = fastest.Genome.SpeedGene,
            FastestHomeRelocationGene = fastest.Genome.HomeRelocationGene,
            FastestWanderGene = fastest.Genome.WanderGene,

            SlowestAge = slowest.Age,
            SlowestEnergy = slowest.Energy,
            SlowestFoodGainGene = slowest.Genome.FoodGainGene,
            SlowestReproductionThresholdGene = slowest.Genome.ReproductionThresholdGene,
            SlowestEyesGene = slowest.Genome.EyesGene,
            SlowestSpeedGene = slowest.Genome.SpeedGene,
            SlowestHomeRelocationGene = slowest.Genome.HomeRelocationGene,
            SlowestWanderGene = slowest.Genome.WanderGene,

            MinEnergyAge = minEnergyOrg.Age,
            MinEnergyEnergy = minEnergyOrg.Energy,
            MinEnergyFoodGainGene = minEnergyOrg.Genome.FoodGainGene,
            MinEnergyReproductionThresholdGene = minEnergyOrg.Genome.ReproductionThresholdGene,
            MinEnergyEyesGene = minEnergyOrg.Genome.EyesGene,
            MinEnergySpeedGene = minEnergyOrg.Genome.SpeedGene,
            MinEnergyHomeRelocationGene = minEnergyOrg.Genome.HomeRelocationGene,
            MinEnergyWanderGene = minEnergyOrg.Genome.WanderGene,

            MaxEnergyAge = maxEnergyOrg.Age,
            MaxEnergyEnergy = maxEnergyOrg.Energy,
            MaxEnergyFoodGainGene = maxEnergyOrg.Genome.FoodGainGene,
            MaxEnergyReproductionThresholdGene = maxEnergyOrg.Genome.ReproductionThresholdGene,
            MaxEnergyEyesGene = maxEnergyOrg.Genome.EyesGene,
            MaxEnergySpeedGene = maxEnergyOrg.Genome.SpeedGene,
            MaxEnergyHomeRelocationGene = maxEnergyOrg.Genome.HomeRelocationGene,
            MaxEnergyWanderGene = maxEnergyOrg.Genome.WanderGene,

            TypicalAge = typical.Age,
            TypicalEnergy = typical.Energy,
            TypicalFoodGainGene = typical.Genome.FoodGainGene,
            TypicalReproductionThresholdGene = typical.Genome.ReproductionThresholdGene,
            TypicalEyesGene = typical.Genome.EyesGene,
            TypicalSpeedGene = typical.Genome.SpeedGene,
            TypicalHomeRelocationGene = typical.Genome.HomeRelocationGene,
            TypicalWanderGene = typical.Genome.WanderGene,
            BiomeStats = biomeStatsList
        };
    }

    public void Tick()
    {
        TickNumber++;

        biomeBirthsLastTick.Clear();
        biomeDeathsLastTick.Clear();

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
            var biome = biomeAtCell[organism.X, organism.Y];
            var baseMetabolism = config.MetabolismBase * biome.MetabolismMultiplier;
            var extraMetabolicLoad = ComputeExtraMetabolicLoad(genome);

            var foodBase = config.FoodGainBase * biome.FoodGainMultiplier;
            var foodFactor = 1.0 + config.FoodGainGeneScale * genome.FoodGainGene;
            if (foodFactor < 0.0) foodFactor = 0.0;
            var energyFromFood = foodBase * foodFactor;

            var reproductionThreshold = config.ReproductionThresholdBase *
                                        (1.0 + config.ReproductionThresholdGeneScale * genome.ReproductionThresholdGene);

            var senseRadius = ComputeSenseRadius(genome);

            // Home-range based energy cost: being far from home is more expensive.
            var extraHomeCost = 0.0;
            if (config.HomeRadius > 0.0 && config.HomeFarCostPerTile > 0.0)
            {
                var distFromHome = ComputeDistanceFromHome(organism);
                if (distFromHome > config.HomeRadius)
                {
                    var effectiveHomeFarCostPerTile = ComputeHomeFarCostPerTile(genome);
                    extraHomeCost = effectiveHomeFarCostPerTile * (distFromHome - config.HomeRadius);
                    organism.TicksFarFromHome++;

                    if (config.HomeRelocationThresholdTicks > 0 &&
                        organism.TicksFarFromHome >= config.HomeRelocationThresholdTicks)
                    {
                        RelocateHomeTowardsCurrentPosition(organism);
                        organism.TicksFarFromHome = 0;
                    }
                }
                else
                {
                    organism.TicksFarFromHome = 0;
                }
            }

            var energyLossPerTick = baseMetabolism + extraMetabolicLoad + extraHomeCost;

            // Energy loss
            organism.Energy -= energyLossPerTick;
            if (organism.Energy <= 0)
            {
                var deathBiome = biomeAtCell[organism.X, organism.Y];
                if (!biomeDeathsLastTick.TryGetValue(deathBiome, out var deathCount))
                {
                    deathCount = 0;
                }
                biomeDeathsLastTick[deathBiome] = deathCount + 1;

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
                var deathBiome = biomeAtCell[organism.X, organism.Y];
                if (!biomeDeathsLastTick.TryGetValue(deathBiome, out var deathCount))
                {
                    deathCount = 0;
                }
                biomeDeathsLastTick[deathBiome] = deathCount + 1;

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
                    HomeX = organism.HomeX,
                    HomeY = organism.HomeY,
                    TicksFarFromHome = 0,
                    Energy = config.StartEnergy,
                    Age = 0,
                    Genome = CloneAndMutateGenome(genome)
                };

                newborns.Add(child);

                var birthBiome = biomeAtCell[child.X, child.Y];
                if (!biomeBirthsLastTick.TryGetValue(birthBiome, out var birthCount))
                {
                    birthCount = 0;
                }
                biomeBirthsLastTick[birthBiome] = birthCount + 1;
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
            SpeedGene = 0.0,
            HomeRelocationGene = 0.0,
            WanderGene = 0.0
        };

    private Genome CloneAndMutateGenome(Genome parent)
    {
        var child = new Genome
        {
            FoodGainGene = MutateGene(parent.FoodGainGene),
            ReproductionThresholdGene = MutateGene(parent.ReproductionThresholdGene),
            EyesGene = MutateGene(parent.EyesGene),
            SpeedGene = MutateGene(parent.SpeedGene),
            HomeRelocationGene = MutateGene(parent.HomeRelocationGene),
            WanderGene = MutateGene(parent.WanderGene)
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

        var homeRelocPositive = Math.Max(0.0, genome.HomeRelocationGene);
        var homeRelocNegative = Math.Min(0.0, genome.HomeRelocationGene);
        extraMetabolicLoad += config.HomeRelocationGeneCostPositive * homeRelocPositive * homeRelocPositive;
        extraMetabolicLoad += config.HomeRelocationGeneCostNegative * homeRelocNegative * homeRelocNegative;

        var wanderPositive = Math.Max(0.0, genome.WanderGene);
        var wanderNegative = Math.Min(0.0, genome.WanderGene);
        extraMetabolicLoad += config.WanderGeneCostPositive * wanderPositive * wanderPositive;
        extraMetabolicLoad += config.WanderGeneCostNegative * wanderNegative * wanderNegative;

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

    private sealed class BiomeAccumulator
    {
        public int Population;
        public double TotalEnergy;
        public double TotalAge;
        public double TotalFoodGainGene;
        public double TotalReproductionThresholdGene;
        public double TotalEyesGene;
        public double TotalSpeedGene;
        public double TotalHomeRelocationGene;
        public double TotalWanderGene;
    }

    private void RegenerateFood()
    {
        for (var x = 0; x < config.Width; x++)
        {
            for (var y = 0; y < config.Height; y++)
            {
                var biome = biomeAtCell[x, y];
                var regenProb = config.FoodRegenProbability * biome.FoodRegenMultiplier;
                if (rng.NextDouble() < regenProb)
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
                var biome = biomeAtCell[x, y];
                var initialDensity = config.InitialFoodDensity * biome.FoodInitialDensityMultiplier;
                if (rng.NextDouble() < initialDensity)
                {
                    food[x, y] += 1.0;
                }
            }
        }
    }

    private double ComputeDistanceFromHome(Organism organism)
    {
        // Use toroidal distance so that wrapping at world edges does not create artificially large distances.
        var dxRaw = Math.Abs(organism.X - organism.HomeX);
        var dyRaw = Math.Abs(organism.Y - organism.HomeY);

        var dx = Math.Min(dxRaw, config.Width - dxRaw);
        var dy = Math.Min(dyRaw, config.Height - dyRaw);

        return Math.Sqrt(dx * dx + dy * dy);
    }

    private void RelocateHomeTowardsCurrentPosition(Organism organism)
    {
        // If a positive relocation factor is configured, move a fraction of the way toward the organism's current position.
        if (config.HomeRelocationFactor > 0.0)
        {
            var effectiveFactor = ComputeHomeRelocationFactor(organism.Genome);
            var newHomeX = (int)Math.Round(organism.HomeX + (organism.X - organism.HomeX) * effectiveFactor);
            var newHomeY = (int)Math.Round(organism.HomeY + (organism.Y - organism.HomeY) * effectiveFactor);

            if (newHomeX < 0) newHomeX += config.Width;
            if (newHomeX >= config.Width) newHomeX -= config.Width;
            if (newHomeY < 0) newHomeY += config.Height;
            if (newHomeY >= config.Height) newHomeY -= config.Height;

            organism.HomeX = newHomeX;
            organism.HomeY = newHomeY;
        }
        else
        {
            // Fallback: move home one step toward the current position on each axis.
            if (organism.HomeX != organism.X)
            {
                organism.HomeX += organism.HomeX < organism.X ? 1 : -1;
            }

            if (organism.HomeY != organism.Y)
            {
                organism.HomeY += organism.HomeY < organism.Y ? 1 : -1;
            }
        }
    }

    private double ComputeHomeRelocationFactor(Genome genome)
    {
        var baseFactor = config.HomeRelocationFactor;
        if (baseFactor <= 0.0)
        {
            return 0.0;
        }

        var multiplier = 1.0 + config.HomeRelocationGeneScale * genome.HomeRelocationGene;
        if (multiplier < 0.1) multiplier = 0.1;
        if (multiplier > 3.0) multiplier = 3.0;

        return baseFactor * multiplier;
    }

    private double ComputeHomeFarCostPerTile(Genome genome)
    {
        var baseCost = config.HomeFarCostPerTile;
        if (baseCost <= 0.0)
        {
            return 0.0;
        }

        var multiplier = 1.0 + config.WanderGeneScale * genome.WanderGene;
        if (multiplier < 0.1) multiplier = 0.1;
        if (multiplier > 3.0) multiplier = 3.0;

        return baseCost * multiplier;
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
                HomeX = x,
                HomeY = y,
                TicksFarFromHome = 0,
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