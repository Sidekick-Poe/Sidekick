using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Repoe.Models.Poe1;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats.Models;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Models.Raw;
namespace Sidekick.Data.Builder.Items;

public class StatBuilder(
    TradeDataProvider tradeDataProvider,
    DataProvider dataProvider,
    RepoeDataProvider repoeDataProvider)
{
    private readonly Regex textHashPattern = new(@"\#");
    private readonly Regex textGameHashPattern = new(@"\{\d+}");

    private readonly Regex newLinePattern = new(@"(?:\\)*[\r\n]+");
    private readonly Regex hashPattern = new(@"\\#");
    private readonly Regex gameHashPattern = new(@"\\{\d+}");

    public async Task Build(IGameLanguage language)
    {
        await Build(GameType.PathOfExile1, language);
        await Build(GameType.PathOfExile2, language);
    }

    private async Task Build(GameType game, IGameLanguage language)
    {
        var definitions = await BuildGameStats(game, language);
        definitions.AddRange(await BuildTradeStats(game, language, definitions));
        definitions = await RemoveIgnoredStats(game, definitions);

        await dataProvider.Write(game, $"stats/{language.Code}.json", definitions);
    }

    private async Task<List<StatDefinition>> BuildGameStats(GameType game, IGameLanguage language)
    {
        var gameStats = await repoeDataProvider.GetStatTranslations(game, language.Code);
        var definitions = new List<StatDefinition>();
        foreach (var gameStat in gameStats)
        {
            definitions.Add(new StatDefinition()
            {
                GameIds = gameStat.Ids,
                GamePatterns = GetGamePatterns(gameStat).ToList(),
                TradePatterns = GetTradePatterns(gameStat).ToList(),
            });
        }

        return definitions;
    }

    private async Task<List<StatDefinition>> BuildTradeStats(GameType game, IGameLanguage language, List<StatDefinition> definitions)
    {
        var apiCategories = await tradeDataProvider.GetRawStats(game, language.Code);
        var apiStats = apiCategories.SelectMany(x => x.Entries);

        var result = new List<StatDefinition>();
        foreach (var apiStat in apiStats)
        {
            if (definitions.Any(def => def.TradePatterns.Any(tp => tp.Id == apiStat.Id))) continue;

            result.Add(new StatDefinition()
            {
                TradePatterns = GetTradePatterns(apiStat).ToList(),
            });
        }

        return result;
    }

    private IEnumerable<GameStatPattern> GetGamePatterns(RepoeStatTranslation gameStat)
    {
        if (gameStat.Languages == null) yield break;

        foreach (var value in gameStat.Languages)
        {
            if (string.IsNullOrEmpty(value.Text)) continue;

            yield return new GameStatPattern()
            {
                Text = GetText(value.Text),
                Option = GetOption(value),
                Negate = value.Handlers?.Any(x => x.Contains("negate")) ?? false,
                Pattern = GetPattern(value.Text),
            };
        }

        yield break;

        int? GetOption(RepoeStatLanguage value)
        {
            if (value.Conditions?.Count != 1) return null;

            if (value.Format?.Count != 1) return null;
            if (value.Format[0] == "#") return null;
            if (value.Format[0] == "ignore") return null;

            var condition = value.Conditions[0];
            if (!condition.Min.HasValue) return null;
            if (condition.Min != condition.Max) return null;

            return (int)condition.Min.Value;
        }
    }

    private IEnumerable<TradeStatPattern> GetTradePatterns(RepoeStatTranslation gameStat)
    {
        if (gameStat.Languages == null) yield break;

        foreach (var tradeStat in gameStat.TradeStats)
        {
            if (string.IsNullOrEmpty(tradeStat.Text)) continue;

            if (tradeStat.Option == null || tradeStat.Option.Options.Count == 0)
            {
                yield return new TradeStatPattern()
                {
                    Text = GetText(tradeStat.Text),
                    Category = GetStatCategory(tradeStat.Type),
                    Id = tradeStat.Id,
                    Pattern = GetPattern(tradeStat.Text),
                };
                continue;
            }

            foreach (var option in tradeStat.Option.Options)
            {
                yield return new TradeStatPattern()
                {
                    Text = GetText(tradeStat.Text, option.Text),
                    Category = GetStatCategory(tradeStat.Type),
                    Id = tradeStat.Id,
                    Pattern = GetPattern(tradeStat.Text, option.Text),
                    Option = new StatOption()
                    {
                        Id = option.Id,
                        Text = option.Text,
                    },
                };
            }
        }
    }

    private IEnumerable<TradeStatPattern> GetTradePatterns(RawTradeStat tradeStat)
    {
        if (tradeStat.Options == null || tradeStat.Options.Options.Count == 0)
        {
            yield return new TradeStatPattern()
            {
                Text = GetText(tradeStat.Text),
                Category = GetStatCategory(tradeStat.Type),
                Id = tradeStat.Id,
                Pattern = GetPattern(tradeStat.Text),
            };
            yield break;
        }

        foreach (var option in tradeStat.Options.Options)
        {
            yield return new TradeStatPattern()
            {
                Text = GetText(tradeStat.Text, option.Text),
                Category = GetStatCategory(tradeStat.Type),
                Id = tradeStat.Id,
                Pattern = GetPattern(tradeStat.Text, option.Text),
                Option = new StatOption()
                {
                    Id = option.Id,
                    Text = option.Text,
                },
            };
        }
    }

    // private IEnumerable<ItemStatDefinition> GetTradeDefinitions(IGameLanguage language, TradeInvariantStats invariantStats, RawTradeStat entry)
    // {
    //     if (entry.Option?.Options.Count > 0)
    //     {
    //         foreach (var option in entry.Option.Options)
    //         {
    //             if (option.Text == null) continue;
    //             yield return new ItemStatDefinition()
    //             {
    //                 Category = statCategory,
    //                 Id = entry.Id,
    //                 Text = GetText(entry.Text, option.Text),
    //                 Pattern = GetPattern(entry.Text, statCategory, option.Text),
    //                 OptionId = option.Id,
    //                 OptionText = option.Text,
    //             };
    //         }
    //     }
    //     else
    //     {
    //         yield return new ItemStatDefinition()
    //         {
    //             Category = statCategory,
    //             Id = entry.Id,
    //             Text = GetText(entry.Text),
    //             Pattern = GetPattern(entry.Text, statCategory),
    //         };
    //     }
    // }

    private string GetText(string text, string? optionText = null)
    {
        text = text.RemoveSquareBrackets();
        text = textGameHashPattern.Replace(text, "#");
        if (optionText == null) return text;

        optionText = optionText.RemoveSquareBrackets();

        var optionLines = new List<string>();
        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            optionLines.Add(textHashPattern.Replace(text, optionLine));
        }

        return string.Join('\n', optionLines).Trim('\r', '\n');
    }

    private Regex GetPattern(string text, string? optionText = null)
    {
        text = text.RemoveSquareBrackets();

        var suffix = @"(?:\ \([a-z]+\))?";

        var patternValue = Regex.Escape(text);
        patternValue = newLinePattern.Replace(patternValue, @"\n");

        if (string.IsNullOrEmpty(optionText))
        {
            patternValue = hashPattern.Replace(patternValue, @"([-+0-9,.]+)");
            patternValue = gameHashPattern.Replace(patternValue, @"([-+0-9,.]+)");
        }
        else
        {
            var optionLines = new List<string>();
            foreach (var optionLine in newLinePattern.Split(optionText))
            {
                optionLines.Add(hashPattern.Replace(patternValue, Regex.Escape(optionLine)) + suffix);
            }

            patternValue = string.Join('\n', optionLines.Where(x => !string.IsNullOrEmpty(x)));
        }

        patternValue = patternValue.Replace(@"\n", suffix + @"\n");// For multiline stats, the category can be suffixed on all lines.
        patternValue += suffix;

        return new Regex($"^{patternValue}$", RegexOptions.None);
    }

    private StatCategory GetStatCategory(string? apiId)
    {
        var value = apiId?.Split('.').First();
        return value.GetEnumFromValue<StatCategory>();
    }

    private async Task<List<StatDefinition>> RemoveIgnoredStats(GameType game, List<StatDefinition> definitions)
    {
        var invariantStats = await tradeDataProvider.GetInvariantStats(game);
        return definitions.Where(IsNotIgnored).ToList();

        bool IsNotIgnored(StatDefinition definition)
        {
            return definition.TradePatterns.All(tp => !invariantStats.IgnoreStatIds.Contains(tp.Id));
        }
    }
}
