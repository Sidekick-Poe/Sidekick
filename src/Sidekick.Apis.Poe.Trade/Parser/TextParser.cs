using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Game;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Sidekick.Game.Parser.Texts;
namespace Sidekick.Apis.Poe.Trade.Parser;

public class TextParser
(
    ILogger<TextParser> logger,
    ISettingsService settingsService,
    GameTextProvider gameTextProvider
) : IInitializableService
{
    private Regex ParanthesesCategory { get; } = new(@"\s*\([a-z]+\)$");
    private Regex AdvancedDigitsFormat { get; } = new(@"([-\d,.]+)\([-+\d,.]+\-[-+\d,.]+\)");
    private Regex AdvancedOptionFormat { get; } = new(@"([-a-zA-Z]+)\s?\([-a-zA-Z\s]+\-[-a-zA-Z\s]+\)");

    private GameType Game { get; set; }

    public int Priority => 100;

    private string? Fractured { get; set; }
    private string? Corrupted { get; set; }
    private string? Desecrated { get; set; }
    private string? Crafted { get; set; }
    private string? Implicit { get; set; }
    private string? Enchant { get; set; }
    private string? Foulborn { get; set; }
    private string? Vestigial { get; set; }

    public async Task Initialize()
    {
        Game = await settingsService.GetGame();

        Fractured = GetKeyword(gameTextProvider.Texts.ModDescriptionFractured);
        Corrupted = GetKeyword(gameTextProvider.Texts.ModDescriptionCorrupted);
        Desecrated = GetKeyword(gameTextProvider.Texts.ModDescriptionDesecrated);
        Crafted = GetKeyword(gameTextProvider.Texts.ModDescriptionCrafted);
        Implicit = GetKeyword(gameTextProvider.Texts.ModDescriptionImplicit);
        Enchant = GetKeyword(gameTextProvider.Texts.ModDescriptionEnchantment);
        Foulborn = GetKeyword(gameTextProvider.Texts.ModDescriptionFoulborn);
        Vestigial = GetKeyword(gameTextProvider.Texts.ModDescriptionVestigial);

        return;

        string? GetKeyword(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            return text.Replace("#", "").Trim();
        }
    }

    public OriginalText? NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        text = StandardizeLineBreaks(text);
        text = RemoveUnusableLine(text);

        var rawText = new OriginalText(text);
        AppendCategoryFromAdvancedLines(rawText);
        RemoveAdvancedMetaLines(rawText);
        CombineLines(rawText);
        CleanupLines(rawText);
        return rawText;

        void CombineLines(OriginalText input)
        {
            foreach (var block in input.Blocks)
            {
                var linesByKey = new Dictionary<(StatCategory Category, string Text), OriginalLine>();
                var combinedLines = new List<OriginalLine>();

                foreach (var line in block.Lines)
                {
                    var key = (line.Category, AdvancedDigitsFormat.Replace(line.Text, "#"));

                    if (!linesByKey.TryGetValue(key, out var previousLine))
                    {
                        linesByKey[key] = line;
                        combinedLines.Add(line);
                        continue;
                    }

                    try
                    {
                        var prevMatch = AdvancedDigitsFormat.Match(previousLine.Text);
                        var currMatch = AdvancedDigitsFormat.Match(line.Text);

                        if (!prevMatch.Success || !currMatch.Success)
                        {
                            combinedLines.Add(line);
                            continue;
                        }

                        var prevValue = decimal.Parse(prevMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                        var currValue = decimal.Parse(currMatch.Groups[1].Value, CultureInfo.InvariantCulture);

                        var sum = prevValue + currValue;
                        var sumText = sum % 1 == 0
                            ? ((int)sum).ToString(CultureInfo.InvariantCulture)
                            : sum.ToString(CultureInfo.InvariantCulture);

                        previousLine.Text = AdvancedDigitsFormat.Replace(previousLine.Text, $"{sumText}(0-999)", 1);
                    }
                    catch (Exception)
                    {
                        logger.LogWarning("Could not parse advanced digits format: {line}", line.Text);
                        combinedLines.Add(line);
                    }
                }

                block.Lines = combinedLines;
            }
        }
    }

    private string StandardizeLineBreaks(string input)
    {
        return Regex.Replace(input, @"[\r\n]+", "\n");
    }

    private string RemoveUnusableLine(string input)
    {
        input = input.Replace(gameTextProvider.Texts.ItemUnusable + "\n" + OriginalText.SeparatorPattern + "\n", string.Empty);
        input = input.Replace(gameTextProvider.Texts.ItemUnusable + "\n", string.Empty);
        return input;
    }

    private void AppendCategoryFromAdvancedLines(OriginalText input)
    {
        foreach (var block in input.Blocks)
        {
            var currentCategory = StatCategory.Explicit;

            foreach (var line in block.Lines)
            {
                if (line.Text.EndsWith("(implicit)")) line.Category = StatCategory.Implicit;
                else if (line.Text.EndsWith("(fractured)")) line.Category = StatCategory.Fractured;
                else if (line.Text.EndsWith("(desecrated)")) line.Category = StatCategory.Desecrated;
                else if (line.Text.EndsWith("(crafted)")) line.Category = StatCategory.Crafted;
                else if (line.Text.EndsWith("(enchant)")) line.Category = StatCategory.Enchant;
                else if (line.Text.EndsWith("(mutated)")) line.Category = StatCategory.Mutated;
                else if (line.Text.EndsWith("(rune)")) line.Category = StatCategory.Rune;

                if (line.Category != StatCategory.Undefined)
                {
                    line.Text = ParanthesesCategory.Replace(line.Text, string.Empty);
                    continue;
                }

                if (line.Text.StartsWith("{") && line.Text.EndsWith("}"))
                {
                    if (Implicit != null && line.Text.Contains(Implicit)) currentCategory = StatCategory.Implicit;
                    else if (Vestigial != null && line.Text.Contains(Vestigial)) currentCategory = StatCategory.Implicit;
                    else if (Fractured != null && line.Text.Contains(Fractured)) currentCategory = StatCategory.Fractured;
                    else if (Desecrated != null && line.Text.Contains(Desecrated)) currentCategory = StatCategory.Desecrated;
                    else if (Crafted != null && line.Text.Contains(Crafted)) currentCategory = StatCategory.Crafted;
                    else if (Enchant != null && line.Text.Contains(Enchant)) currentCategory = StatCategory.Enchant;
                    else if (Foulborn != null && line.Text.Contains(Foulborn)) currentCategory = StatCategory.Mutated;
                    else if (Corrupted != null && line.Text.Contains(Corrupted))
                    {
                        if (Game == GameType.PathOfExile1) currentCategory = StatCategory.Implicit;
                        else if (Game == GameType.PathOfExile2) currentCategory = StatCategory.Enchant;
                    }
                    else currentCategory = StatCategory.Explicit;
                }

                line.Category = currentCategory;
            }
        }
    }

    private void RemoveAdvancedMetaLines(OriginalText input)
    {
        foreach (var block in input.Blocks)
        {
            block.Lines.RemoveAll(x => x.Text.StartsWith("{"));
            block.Lines.RemoveAll(x => x.Text.StartsWith("("));
        }
    }

    void CleanupLines(OriginalText input)
    {
        foreach (var block in input.Blocks)
        {
            foreach (var line in block.Lines)
            {
                // Removes text like ' — Unscalable Value'
                line.Text = line.Text.Split(" — ")[0];

                // Remove range parantheses
                line.Text = AdvancedDigitsFormat.Replace(line.Text, "$1");
                line.Text = AdvancedOptionFormat.Replace(line.Text, "$1");
            }
        }
    }
}
