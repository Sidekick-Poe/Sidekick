using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Trade.Parser.Definition;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo;
using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser;

public class ItemParser
(
    ILogger<ItemParser> logger,
    IStatParser statParser,
    IPseudoParser pseudoParser,
    IPropertyParser propertyParser,
    ICurrentGameLanguage currentGameLanguage,
    IItemDefinitionParser itemDefinitionParser,
    ISettingsService settingsService
) : IItemParser
{
    private Regex? UnusablePattern { get; set; }
    private Regex AdvancedDigitsFormat { get; } = new(@"([-+\d,.]+)\([-+\d,.]+\-[-+\d,.]+\)");

    public int Priority => 100;

    private GameType Game { get; set; }

    public async Task Initialize()
    {
        var unusableRegex = Regex.Escape(currentGameLanguage.Language.DescriptionUnusable);
        unusableRegex += @"\n+" + RawText.SeparatorPattern + @"\n+";
        UnusablePattern = new Regex(unusableRegex, RegexOptions.Compiled);
        Game = await settingsService.GetGame();
    }

    public Item ParseItem(string? text)
    {
        if (string.IsNullOrEmpty(text)) throw new UnparsableException(text);

        try
        {
            text = NormalizeText(text);

            var item = new Item(Game, currentGameLanguage.Language, text);

            // Rarity property is required for definition parsing. This means that it must be parsed first.
            propertyParser.GetDefinition<RarityProperty>().Parse(item);

            itemDefinitionParser.Parse(item);
            propertyParser.Parse(item);
            statParser.Parse(item);
            propertyParser.ParseAfterStats(item);
            pseudoParser.Parse(item);

            return item;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw new UnparsableException(text);
        }
    }

    private string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        text = StandardizeLineBreaks(text);
        text = RemoveUnusableLine(text);
        text = RemoveAdvancedMetaLines(text);
        text = CombineLines(text);
        text = RemoveNumericParentheses(text);
        return text;

        string StandardizeLineBreaks(string input)
        {
            return Regex.Replace(input, @"[\r\n]+", "\n");
        }

        string RemoveUnusableLine(string input)
        {
            return UnusablePattern?.Replace(input, string.Empty) ?? input;
        }

        string RemoveAdvancedMetaLines(string input)
        {
            var cleaned = new List<string>();
            foreach (var line in input.Split('\n'))
            {
                if (line.StartsWith("{") && line.EndsWith("}")) continue;
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

        string RemoveNumericParentheses(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return AdvancedDigitsFormat.Replace(input, "$1");
        }
    }
}
