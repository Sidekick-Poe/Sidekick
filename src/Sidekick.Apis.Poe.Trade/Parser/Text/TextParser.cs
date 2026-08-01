using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Texts;

namespace Sidekick.Apis.Poe.Trade.Parser.Text;

public class TextParser
(
    ILogger<TextParser> logger,
    ICurrentGameLanguage currentGameLanguage,
    ISettingsService settingsService,
    DataProvider dataProvider
) : IInitializableService
{
    private Regex? UnusablePattern { get; set; }
    private Regex AdvancedDigitsFormat { get; } = new(@"([-\d,.]+)\([-+\d,.]+\-[-+\d,.]+\)");
    private Regex AdvancedOptionFormat { get; } = new(@"([-a-zA-Z]+)\([-a-zA-Z\s]+\-[-a-zA-Z\s]+\)");

    private GameType Game { get; set; }

    public int Priority => 100;

    private string? Fractured { get; set; }
    private string? Corrupted { get; set; }
    private string? Desecrated { get; set; }
    private string? Crafted { get; set; }
    private string? Implicit { get; set; }
    private string? Enchant { get; set; }
    private string? Foulborn { get; set; }

    public async Task Initialize()
    {
        Game = await settingsService.GetGame();
        var texts = await dataProvider.Read<DataText>(Game, DataType.Texts, currentGameLanguage.Language);
        Fractured = texts.ModDescriptionFractured?.Replace("#", "").Trim();
        Corrupted = texts.ModDescriptionCorrupted?.Replace("#", "").Trim();
        Desecrated = texts.ModDescriptionDesecrated?.Replace("#", "").Trim();
        Crafted = texts.ModDescriptionCrafted?.Replace("#", "").Trim();
        Implicit = texts.ModDescriptionImplicit?.Replace("#", "").Trim();
        Enchant = texts.ModDescriptionEnchantment?.Replace("#", "").Trim();
        Foulborn = texts.ModDescriptionFoulborn?.Replace("#", "").Trim();

        var unusableRegex = Regex.Escape(currentGameLanguage.Language.DescriptionUnusable);
        unusableRegex += @"\n+" + RawText.SeparatorPattern + @"\n+";
        UnusablePattern = new Regex(unusableRegex, RegexOptions.Compiled);
    }

    public string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        text = StandardizeLineBreaks(text);
        text = RemoveUnusableLine(text);
        text = AppendCategoryFromAdvancedLines(text);
        text = RemoveAdvancedMetaLines(text);
        text = CombineLines(text);
        text = RemoveParentheses(text);
        text = RemoveDashedMetaString(text);
        return text;

        string StandardizeLineBreaks(string input)
        {
            return Regex.Replace(input, @"[\r\n]+", "\n");
        }

        string RemoveUnusableLine(string input)
        {
            return UnusablePattern?.Replace(input, string.Empty) ?? input;
        }

        string AppendCategoryFromAdvancedLines(string input)
        {
            var lines = input.Split('\n');
            var result = new List<string>();
            string? currentSuffix = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("{") && line.EndsWith("}"))
                {
                    if (Implicit != null && line.Contains(Implicit)) currentSuffix = "(implicit)";
                    else if (Fractured != null && line.Contains(Fractured)) currentSuffix = "(fractured)";
                    else if (Desecrated != null && line.Contains(Desecrated)) currentSuffix = "(desecrated)";
                    else if (Crafted != null && line.Contains(Crafted)) currentSuffix = "(crafted)";
                    else if (Enchant != null && line.Contains(Enchant)) currentSuffix = "(enchant)";
                    else if (Foulborn != null && line.Contains(Foulborn)) currentSuffix = "(mutated)";
                    else if (Corrupted != null && line.Contains(Corrupted))
                    {
                        if (Game == GameType.PathOfExile1) currentSuffix = "(implicit)";
                        else if (Game == GameType.PathOfExile2) currentSuffix = "(enchant)";
                    }
                    else currentSuffix = null;

                    result.Add(line);
                    continue;
                }

                if (currentSuffix != null && !string.IsNullOrWhiteSpace(line) && line != RawText.SeparatorPattern)
                    result.Add($"{line} {currentSuffix}");
                else
                    result.Add(line);
            }

            return string.Join('\n', result);
        }

        string RemoveAdvancedMetaLines(string input)
        {
            var cleaned = new List<string>();
            foreach (var line in input.Split('\n'))
            {
                if (line.StartsWith("{") && line.EndsWith("}")) continue;
                if (line.StartsWith("(")) continue;
                cleaned.Add(line);
            }

            return string.Join('\n', cleaned);
        }

        string CombineLines(string input)
        {
            var dictionary = new Dictionary<string, int>();
            var split = input.Split('\n');
            var output = new List<string>();

            for (var i = 0; i < split.Length; i++)
            {
                var line = split[i];
                if (line == RawText.SeparatorPattern)
                {
                    output.Add(line);
                    continue;
                }

                var key = AdvancedDigitsFormat.Replace(line, "#");
                if (dictionary.TryAdd(key, output.Count))
                {
                    output.Add(line);
                    continue;
                }

                try
                {
                    var previousIndex = dictionary[key];
                    var previousLine = output[previousIndex];

                    var prevMatch = AdvancedDigitsFormat.Match(previousLine);
                    var currMatch = AdvancedDigitsFormat.Match(line);

                    if (!prevMatch.Success || !currMatch.Success)
                    {
                        output.Add(line);
                        continue;
                    }

                    var prevValue = decimal.Parse(prevMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                    var currValue = decimal.Parse(currMatch.Groups[1].Value, CultureInfo.InvariantCulture);

                    var sum = prevValue + currValue;
                    var sumText = sum % 1 == 0 ? ((int)sum).ToString(CultureInfo.InvariantCulture) : sum.ToString(CultureInfo.InvariantCulture);

                    output[previousIndex] = AdvancedDigitsFormat.Replace(previousLine, $"{sumText}(0-999)", 1);
                }
                catch (Exception)
                {
                    logger.LogWarning("Could not parse advanced digits format: {line}", line);
                    output.Add(line);
                }
            }

            return string.Join('\n', output);
        }

        string RemoveParentheses(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            input = AdvancedDigitsFormat.Replace(input, "$1");
            return AdvancedOptionFormat.Replace(input, "$1");
        }

        // Removes text like ' — Unscalable Value'
        string RemoveDashedMetaString(string input)
        {
            if (!input.Contains(" — ")) return input;

            var cleaned = input
                .Split('\n')
                .Select(line => line.Split(" — ")[0]);

            return string.Join('\n', cleaned);
        }
    }
}
