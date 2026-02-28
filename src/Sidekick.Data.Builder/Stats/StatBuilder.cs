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
    private readonly Regex textLocalPattern = new(@"\ \(Local\)$");

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

    private async Task<List<StatPattern>> BuildGameStats(GameType game, IGameLanguage language)
    {
        var gameStats = await repoeDataProvider.GetStatTranslations(game, language.Code);
        var definitions = new List<StatPattern>();
        foreach (var gameStat in gameStats)
        {
            definitions.AddRange(GetGamePatterns(gameStat));
        }

        return definitions;
    }

    private async Task<List<StatPattern>> BuildTradeStats(GameType game, IGameLanguage language, List<StatPattern> definitions)
    {
        var apiCategories = await tradeDataProvider.GetRawStats(game, language.Code);
        var apiStats = apiCategories.SelectMany(x => x.Entries);

        var result = new List<StatPattern>();
        foreach (var apiStat in apiStats)
        {
            if (definitions.Any(def => def.TradeIds.Contains(apiStat.Id))) continue;

            result.AddRange(GetTradePatterns(apiStat));
        }

        return result;
    }

    private IEnumerable<StatPattern> GetGamePatterns(RepoeStatTranslation gameStat)
    {
        if (gameStat.Languages == null) yield break;

        foreach (var language in gameStat.Languages)
        {
            if (string.IsNullOrEmpty(language.Text)) continue;

            var text = GetText(language.Text);
            var value = GetValue(language);
            var tradeStats = GetTradeStats(value).ToList();

            yield return new StatPattern()
            {
                Source = StatSource.Game,
                Category = GetGameStatCategory(tradeStats),
                GameIds = gameStat.Ids,
                TradeIds = tradeStats.Select(x => x.Id).ToList(),
                Text = GetText(language.Text),
                Option = GetOption(tradeStats, value),
                Negate = language.Handlers?.Any(x => x.Contains("negate")) ?? false,
                Pattern = GetPattern(language.Text),
                Value = GetValue(language),
                LineCount = text.Split('\n').Length,
            };
        }

        yield break;

        IEnumerable<RepoeStatTrade> GetTradeStats(int? value)
        {
            foreach (var tradeStat in gameStat.TradeStats)
            {
                if (value == null)
                {
                    if (tradeStat.Options == null) yield return tradeStat;
                    continue;
                }

                if (tradeStat.Options == null) continue;

                if (tradeStat.Options.Options.Any(x => x.Id == value))
                {
                    yield return tradeStat;
                }
            }
        }

        StatOption? GetOption(List<RepoeStatTrade> tradeStats, int? value)
        {
            var tradeOption = tradeStats
                .Where(x => x.Options != null)
                .SelectMany(x => x.Options!.Options)
                .FirstOrDefault(x => x.Id == value);
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

        int? GetValue(RepoeStatLanguage value)
        {
            if (value.Conditions?.Count != 1) return null;

            if (value.Format?.Count != 1) return null;
            if (value.Format[0] != "ignore") return null;

            var condition = value.Conditions[0];
            if (!condition.Min.HasValue || !condition.Max.HasValue) return null;
            if (Math.Abs(condition.Min.Value - condition.Max.Value) > 0.1) return null;

            return (int)condition.Min.Value;
        }
    }

    private IEnumerable<StatPattern> GetTradePatterns(RawTradeStat tradeStat)
    {
        if (tradeStat.Options == null || tradeStat.Options.Options.Count == 0)
        {
            var text = GetText(tradeStat.Text);

            yield return new StatPattern()
            {
                Source = StatSource.Trade,
                Text = text,
                Category = GetStatCategory(tradeStat.Type),
                TradeIds = [tradeStat.Id],
                Pattern = GetPattern(tradeStat.Text),
                LineCount = text.Split('\n').Length,
            };
            yield break;
        }

        foreach (var option in tradeStat.Options.Options)
        {
            var text = GetText(tradeStat.Text, option.Text);

            yield return new StatPattern()
            {
                Source = StatSource.Trade,
                Text = text,
                Category = GetStatCategory(tradeStat.Type),
                TradeIds = [tradeStat.Id],
                Pattern = GetPattern(tradeStat.Text, option.Text),
                LineCount = text.Split('\n').Length,
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
        text = textLocalPattern.Replace(text, string.Empty);
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
        text = textLocalPattern.Replace(text, string.Empty);

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
