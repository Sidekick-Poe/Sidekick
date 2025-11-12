namespace Sidekick.Modules.Trade.Tools.Chromatic;

// This file has been adapted and translated to C# from the original JavaScript implementation
// available in the following repository: https://github.com/Siveran/siveran.github.io
// Credit goes to the original author(s) for the inspiration and foundational logic.

public static class ChromaticProbabilityCalculator
{
    public static List<ProbabilityResult> Calculate(int totalSockets, int strength, int dexterity, int intelligence, int red, int green, int blue)
    {
        // Validate inputs
        if (totalSockets < 1 || totalSockets > 6 || red < 0 || green < 0 || blue < 0 || red + green + blue > totalSockets)
        {
            return
            [
                new ProbabilityResult("Error! Invalid input values.", 0, 1),
            ];
        }

        // Core logic similar to "Main.getProbabilities" from the JavaScript file
        var requirements = new ColoredStats(strength, dexterity, intelligence);
        var desired = new ColoredStats(red, green, blue);
        var colorChances = GetColorChances(requirements);

        var results = new List<ProbabilityResult>();
        foreach (var recipe in Recipes)
        {
            if (recipe.Red > red || recipe.Green > green || recipe.Blue > blue) continue;

            var probability = CalculateProbability(colorChances, desired, recipe, totalSockets);
            results.Add(probability);
        }

        return results.OrderBy(x => x.AverageCost).ToList();
    }

    private static ColorChances GetColorChances(ColoredStats stats)
    {
        const double x = 5;// Base value for calculations
        const double c = 5;// Core constant value
        const double maxOnColorChance = 0.9;// Maximum chance value for a dominant color

        var totalRequirements = stats.Red + stats.Green + stats.Blue;// Sum of the stats
        var numberOfRequirements = CountNonZero();

        // Map each stat into respective chances
        var redChance = RequirementToChance(stats.Red);
        var greenChance = RequirementToChance(stats.Green);
        var blueChance = RequirementToChance(stats.Blue);

        return new ColorChances(redChance, greenChance, blueChance);

        int CountNonZero()
        {
            var count = 0;
            if (stats.Red > 0) count++;
            if (stats.Green > 0) count++;
            if (stats.Blue > 0) count++;
            return count;
        }

        // Function to calculate individual color chance
        double RequirementToChance(int requirement)
        {
            switch (numberOfRequirements)
            {
                case 1:
                    return requirement > 0
                        ? maxOnColorChance * (x + c + requirement) / (totalRequirements + 3 * x + c)
                        : ((1 - maxOnColorChance) / 2) + (maxOnColorChance * x / (totalRequirements + 3 * x + c));
                case 2:
                    return requirement > 0
                        ? maxOnColorChance * requirement / totalRequirements
                        : (1 - maxOnColorChance);
                case 3:
                    return (double)requirement / totalRequirements;
                default:
                    return 0.0;// Return 0 for invalid cases
            }
        }
    }

    private static ProbabilityResult CalculateProbability(ColorChances chances, ColoredStats desired, Recipe recipe, int totalSockets)
    {
        // Step 1: Calculate remaining colors after recipe deduction
        var unvoricifiedDesires = new ColoredStats(desired.Red - recipe.Red, desired.Green - recipe.Green, desired.Blue - recipe.Blue);
        var remainingSockets = totalSockets - desired.Red - desired.Green - desired.Blue;

        // Step 2: Calculate the base multinomial probability for the remaining sockets
        var baseProbability = Multinomial(unvoricifiedDesires, remainingSockets);

        // Step 3: Adjust the probability for "Chromatic" recipes if applicable
        if (recipe.Description == "Chromatic")
        {
            var chromaticBonus = CalculateChromaticBonus(chances, desired, totalSockets);
            baseProbability /= (1 - chromaticBonus);
        }

        return new ProbabilityResult(recipe.Description, recipe.Cost, baseProbability);

        double Multinomial(ColoredStats multinomialDesired, int freeSockets)
        {
            if (freeSockets > 0)
            {
                // Recur for allocating free sockets across colors
                return Multinomial(multinomialDesired with
                       {
                           Red = multinomialDesired.Red + 1
                       }, freeSockets - 1) * chances.RedChance
                       + Multinomial(multinomialDesired with
                       {
                           Green = multinomialDesired.Green + 1
                       }, freeSockets - 1) * chances.GreenChance
                       + Multinomial(multinomialDesired with
                       {
                           Blue = multinomialDesired.Blue + 1
                       }, freeSockets - 1) * chances.BlueChance;
            }

            // Base case: Calculate multinomial coefficient and final probability
            return Factorial(multinomialDesired.Red + multinomialDesired.Green + multinomialDesired.Blue)
                   / (Factorial(multinomialDesired.Red) * Factorial(multinomialDesired.Green) * Factorial(multinomialDesired.Blue))
                   * Math.Pow(chances.RedChance, multinomialDesired.Red)
                   * Math.Pow(chances.GreenChance, multinomialDesired.Green)
                   * Math.Pow(chances.BlueChance, multinomialDesired.Blue);
        }
    }

    private static double CalculateChromaticBonus(ColorChances chances, ColoredStats desired, int freeSockets, ColoredStats rolled = null, int position = 1)
    {
        rolled ??= new ColoredStats(0, 0, 0);

        // If all colors are satisfied, no bonus is needed
        if (rolled.Red >= desired.Red && rolled.Green >= desired.Green && rolled.Blue >= desired.Blue)
        {
            return 0;
        }
        // Spread the chromatic bonus among available free sockets
        if (freeSockets > 0)
        {
            return (position <= 1 ? CalculateChromaticBonus(chances, desired, freeSockets - 1,
                                                            new ColoredStats(rolled.Red + 1, rolled.Green, rolled.Blue), 1) : 0)
                   + (position <= 2 ? CalculateChromaticBonus(chances, desired, freeSockets - 1,
                                                              new ColoredStats(rolled.Red, rolled.Green + 1, rolled.Blue), 2) : 0)
                   + CalculateChromaticBonus(chances, desired, freeSockets - 1,
                                             new ColoredStats(rolled.Red, rolled.Green, rolled.Blue + 1), 3);
        }

        // Base case: Calculate bonus probability using factorials and color chances
        return Factorial(rolled.Red + rolled.Green + rolled.Blue)
               / (Factorial(rolled.Red) * Factorial(rolled.Green) * Factorial(rolled.Blue))
               * Math.Pow(chances.RedChance, rolled.Red * 2)
               * Math.Pow(chances.GreenChance, rolled.Green * 2)
               * Math.Pow(chances.BlueChance, rolled.Blue * 2);
    }

    private static double Factorial(int n)
    {
        if (n <= 1) return 1;
        var result = 1;
        for (var i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }

    private static List<Recipe> Recipes => new()
    {
        new Recipe(0, 0, 0, 1, "Drop Rate"),
        new Recipe(0, 0, 0, 1, "Chromatic"),
        new Recipe(1, 0, 0, 4, "1R"),
        new Recipe(0, 1, 0, 4, "1G"),
        new Recipe(0, 0, 1, 4, "1B"),
        new Recipe(2, 0, 0, 25, "2R"),
        new Recipe(0, 2, 0, 25, "2G"),
        new Recipe(0, 0, 2, 25, "2B"),
        new Recipe(0, 1, 1, 15, "1G1B"),
        new Recipe(1, 0, 1, 15, "1R1B"),
        new Recipe(1, 1, 0, 15, "1R1G"),
        new Recipe(3, 0, 0, 120, "3R"),
        new Recipe(0, 3, 0, 120, "3G"),
        new Recipe(0, 0, 3, 120, "3B"),
        new Recipe(2, 1, 0, 100, "2R1G"),
        new Recipe(2, 0, 1, 100, "2R1B"),
        new Recipe(1, 2, 0, 100, "1R2G"),
        new Recipe(0, 2, 1, 100, "2G1B"),
        new Recipe(1, 0, 2, 100, "1R2B"),
        new Recipe(0, 1, 2, 100, "1G2B")
    };
}

public record ColoredStats(int Red, int Green, int Blue);

public record ColorChances(double RedChance, double GreenChance, double BlueChance);

public record ProbabilityResult(string Description, int Cost, double BaseProbability)
{
    public double ChancePercentage => BaseProbability * 100;
    public double AverageCost => Cost <= 0 ? 0 : Cost / BaseProbability;
    public double AverageTries => 1 / BaseProbability;
    public double StandardDeviation => Math.Sqrt((1 - BaseProbability) / (BaseProbability * BaseProbability));
}

public record Recipe(int Red, int Green, int Blue, int Cost, string Description);
