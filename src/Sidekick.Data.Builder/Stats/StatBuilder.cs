using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Data.Builder.Repoe;
using Sidekick.Data.Builder.Repoe.Models.Poe1;
using Sidekick.Data.Extensions;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Stats;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Raw;
namespace Sidekick.Data.Builder.Stats;

public class StatBuilder(
    DataProvider dataProvider,
    RepoeDownloader repoeDownloader,
    IFuzzyService fuzzyService)
{
    private record TradeReplaceEntry(Regex Pattern, string Replacement);

    private readonly Regex textHashPattern = new(@"\#");
    private readonly Regex textGameHashPattern = new(@"\{\d+}");
    private readonly Regex textLocalPattern = new(@"\ \(Local\)$");

    private readonly Regex newLinePattern = new(@"(?:\\)*[\r\n]+");
    private readonly Regex hashPattern = new(@"\\#");
    private readonly Regex gameHashPattern = new(@"\\{\d+}");
    private readonly Regex parenthesesPattern = new(@"(\\\ *\\\([^\(\)]*\\\))");

    private List<TradeReplaceEntry> TradeReplacePatterns { get; set; } = [];

    public async Task Build(IGameLanguage language)
    {
        TradeReplacePatterns = BuildReplacementPatterns(language);

        await Build(GameType.PathOfExile1, language);
        await Build(GameType.PathOfExile2, language);
    }

    private async Task Build(GameType game, IGameLanguage language)
    {
        var definitions = await BuildGameStats(game, language);
        definitions.AddRange(await BuildTradeStats(game, language, definitions));

        var invariantStats = await dataProvider.Read<TradeInvariantStats>(game, DataType.TradeStats);
        ComputeSpecialPseudoPattern(definitions, invariantStats.IncursionRoomStatIds);
        RemoveSpecialPseudoPattern(definitions, invariantStats.IncursionRoomStatIds, x => x.Option?.Id == 2);
        ComputeSpecialPseudoPattern(definitions, invariantStats.LogbookFactionStatIds);

        await dataProvider.Write(game, DataType.Stats, language, definitions);
    }

    private async Task<List<StatDefinition>> BuildGameStats(GameType game, IGameLanguage language)
    {
        var gameStats = await repoeDownloader.ReadStatTranslations(game, language.Code);
        var definitions = new List<StatDefinition>();
        foreach (var gameStat in gameStats)
        {
            definitions.AddRange(GetGamePatterns(gameStat));
        }

        return definitions;
    }

    private async Task<List<StatDefinition>> BuildTradeStats(GameType game, IGameLanguage language, List<StatDefinition> definitions)
    {
        var apiCategories = await dataProvider.Read<RawTradeResult<List<RawTradeStatCategory>>>(game, DataType.TradeRawStats, language);
        var apiStats = apiCategories.Result.SelectMany(x => x.Entries);

        var result = new List<StatDefinition>();
        foreach (var apiStat in apiStats)
        {
            if (definitions.Any(def => def.TradeIds.Contains(apiStat.Id))) continue;

            result.AddRange(GetTradePatterns(language, apiStat));
        }

        return result;
    }

    private IEnumerable<StatDefinition> GetGamePatterns(RepoeStatTranslation gameStat)
    {
        if (gameStat.Languages == null) yield break;

        foreach (var language in gameStat.Languages)
        {
            if (string.IsNullOrEmpty(language.Text)) continue;

            var text = GetText(language.Text);
            var value = GetValue(language);
            var tradeStats = GetTradeStats(value).ToList();

            yield return new StatDefinition()
            {
                Source = StatSource.Game,
                Category = GetGameStatCategory(tradeStats),
                GameIds = gameStat.Ids,
                TradeIds = tradeStats.Select(x => x.Id).ToList(),
                Text = text,
                Option = GetOption(tradeStats, value),
                Negate = language.Handlers?.Any(x => x.Contains("negate")) ?? false,
                Pattern = GetPattern(language.Text),
                Value = value,
                LineCount = text.Split('\n').Length,
            };
        }

        yield break;

        IEnumerable<RepoeStatTrade> GetTradeStats(int? value)
        {
            var hasOptions = gameStat.TradeStats.Any(x => x.Options != null);
            foreach (var tradeStat in gameStat.TradeStats)
            {
                if (!hasOptions)
                {
                    yield return tradeStat;
                    continue;
                }

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
            if (!condition.Min.HasValue) return null;
            if (condition.Max.HasValue && Math.Abs(condition.Min.Value - condition.Max.Value) > 0.1) return null;

            return (int)condition.Min.Value;
        }
    }

    private IEnumerable<StatDefinition> GetTradePatterns(IGameLanguage gameLanguage, RawTradeStat tradeStat)
    {
        if (tradeStat.Options == null || tradeStat.Options.Options.Count == 0)
        {
            var text = GetText(tradeStat.Text);

            yield return new StatDefinition()
            {
                Source = StatSource.Trade,
                Text = text,
                FuzzyText = GetFuzzyText(gameLanguage, tradeStat.Text),
                Category = GetStatCategory(tradeStat.Type),
                TradeIds = [tradeStat.Id],
                Pattern = GetPattern(tradeStat.Text, replacePatterns: true),
                LineCount = text.Split('\n').Length,
            };
            yield break;
        }

        foreach (var option in tradeStat.Options.Options)
        {
            var text = GetText(tradeStat.Text, option.Text);

            yield return new StatDefinition()
            {
                Source = StatSource.Trade,
                Text = text,
                FuzzyText = GetFuzzyText(gameLanguage, tradeStat.Text, option.Text),
                Category = GetStatCategory(tradeStat.Type),
                TradeIds = [tradeStat.Id],
                Pattern = GetPattern(tradeStat.Text, option.Text, replacePatterns: true),
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

    private string GetFuzzyText(IGameLanguage language, string text, string? optionText = null)
    {
        if (string.IsNullOrEmpty(optionText))
        {
            return fuzzyService.CleanFuzzyText(language, text);
        }

        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            text = textHashPattern.Replace(text, optionLine);
        }

        return fuzzyService.CleanFuzzyText(language, text);
    }

    private Regex GetPattern(string text, string? optionText = null, bool replacePatterns = false)
    {
        text = text.RemoveSquareBrackets();
        text = textLocalPattern.Replace(text, string.Empty);

        var suffix = @"(?:\ \([a-z]+\))?";

        var patternValue = Regex.Escape(text);
        patternValue = newLinePattern.Replace(patternValue, @"\n");
        patternValue = parenthesesPattern.Replace(patternValue, "(?:$1)?");

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

        if (replacePatterns)
        {
            foreach (var replaceEntry in TradeReplacePatterns)
            {
                patternValue = replaceEntry.Pattern.Replace(patternValue, replaceEntry.Replacement);
            }
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

    private List<TradeReplaceEntry> BuildReplacementPatterns(IGameLanguage language)
    {
        List<TradeReplaceEntry> result = [];
        if (!string.IsNullOrEmpty(language.RegexIncreased))
        {
            result.Add(new(
                       new Regex(language.RegexIncreased),
                       $"(?:{language.RegexIncreased}|{language.RegexReduced})"));
        }

        if (!string.IsNullOrEmpty(language.RegexMore))
        {
            result.Add(new(
                       new Regex(language.RegexMore),
                       $"(?:{language.RegexMore}|{language.RegexLess})"));
        }

        if (!string.IsNullOrEmpty(language.RegexFaster))
        {
            result.Add(new(
                       new Regex(language.RegexFaster),
                       $"(?:{language.RegexFaster}|{language.RegexSlower})"));
        }

        return result;
    }

    private void ComputeSpecialPseudoPattern(List<StatDefinition> definitions, List<string> patternIds)
    {
        var patterns = (from definition in definitions
            where definition.Category == StatCategory.Pseudo
            where definition.TradeIds.Any(patternIds.Contains)
            select definition);

        foreach (var pattern in patterns)
        {
            pattern.Pattern = GetPattern(pattern.Text.Split(':', 2).Last().Trim());
        }
    }

    private void RemoveSpecialPseudoPattern(List<StatDefinition> definitions, List<string> patternIds, Func<StatDefinition, bool> predicate)
    {
        definitions.RemoveAll(x => x.TradeIds.Any(patternIds.Contains) && predicate(x));
    }
}
