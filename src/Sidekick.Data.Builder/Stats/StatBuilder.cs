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
namespace Sidekick.Data.Builder.Stats;

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
                Patterns = GetGamePatterns(gameStat).ToList(),
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
            if (definitions.Any(def => def.Patterns.Any(pattern => pattern.TradeIds.Contains(apiStat.Id)))) continue;

            result.Add(new StatDefinition()
            {
                Patterns = GetTradePatterns(apiStat).ToList(),
            });
        }

        return result;
    }

    private IEnumerable<StatPattern> GetGamePatterns(RepoeStatTranslation gameStat)
    {
        if (gameStat.Languages == null) yield break;

        foreach (var value in gameStat.Languages)
        {
            if (string.IsNullOrEmpty(value.Text)) continue;

            var optionId = GetGameOption(value);
            var tradeStats = GetTradeStats(optionId).ToList();

            yield return new StatPattern()
            {
                Source = StatSource.Game,
                Category = GetGameStatCategory(tradeStats),
                TradeIds = tradeStats.Select(x => x.Id).ToList(),
                Text = GetText(value.Text),
                Option = GetOption(tradeStats),
                Negate = value.Handlers?.Any(x => x.Contains("negate")) ?? false,
                Pattern = GetPattern(value.Text),
                Value = GetValue(value),
            };
        }

        yield break;

        int? GetGameOption(RepoeStatLanguage value)
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

        IEnumerable<RepoeStatTrade> GetTradeStats(int? optionId)
        {
            foreach (var tradeStat in gameStat.TradeStats)
            {
                if (optionId == null)
                {
                    if (tradeStat.Options == null) yield return tradeStat;
                    continue;
                }

                if (tradeStat.Options == null) continue;

                if (tradeStat.Options.Options.Any(x => x.Id == optionId))
                {
                    yield return tradeStat;
                }
            }
        }

        StatOption? GetOption(List<RepoeStatTrade> tradeStats)
        {
            var tradeOption = tradeStats
                .Where(x => x.Options != null)
                .SelectMany(x => x.Options!.Options)
                .FirstOrDefault();
            if (tradeOption == null) return null;

            return new StatOption()
            {
                Id = tradeOption.Id,
                Text = tradeOption.Text,
            };
        }

        StatCategory GetGameStatCategory(List<RepoeStatTrade> tradeStats)
        {
            var categories = tradeStats.Select(x => GetStatCategory(x.Type)).Distinct().ToList();
            return categories.Count != 1 ? StatCategory.Undefined : categories[0];
        }

        double? GetValue(RepoeStatLanguage value)
        {
            if (value.Conditions?.Count != 1) return null;

            if (value.Format?.Count != 1) return null;
            if (value.Format[0] != "ignore") return null;

            var condition = value.Conditions[0];
            if (!condition.Min.HasValue) return null;
            if (condition.Min != condition.Max) return null;

            return condition.Min.Value;
        }
    }

    private IEnumerable<StatPattern> GetTradePatterns(RawTradeStat tradeStat)
    {
        if (tradeStat.Options == null || tradeStat.Options.Options.Count == 0)
        {
            yield return new StatPattern()
            {
                Source = StatSource.Trade,
                Text = GetText(tradeStat.Text),
                Category = GetStatCategory(tradeStat.Type),
                TradeIds = [tradeStat.Id],
                Pattern = GetPattern(tradeStat.Text),
            };
            yield break;
        }

        foreach (var option in tradeStat.Options.Options)
        {
            yield return new StatPattern()
            {
                Source = StatSource.Trade,
                Text = GetText(tradeStat.Text, option.Text),
                Category = GetStatCategory(tradeStat.Type),
                TradeIds = [tradeStat.Id],
                Pattern = GetPattern(tradeStat.Text, option.Text),
                Option = new StatOption()
                {
                    Id = option.Id,
                    Text = option.Text,
                },
            };
        }
    }

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
}
