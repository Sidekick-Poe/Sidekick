using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Raw;
namespace Sidekick.Data.Builder.Trade;

public class TradeStatBuilder(
    DataProvider dataProvider)
{
    private readonly Regex parseHashPattern = new("\\#");
    private readonly Regex newLinePattern = new(@"(?:\\)*[\r\n]+");

    public async Task Build(IGameLanguage language)
    {
        await Build(GameType.PathOfExile1, language);
        await Build(GameType.PathOfExile2, language);
    }

    private async Task Build(GameType game, IGameLanguage language)
    {
        var apiCategories = await dataProvider.Read<RawTradeResult<List<RawTradeStatCategory>>>(game, DataType.TradeRawStats, language);
        var invariantStats = await dataProvider.Read<TradeInvariantStats>(game, DataType.TradeInvariantStats);

        var definitions = new List<TradeStatDefinition>();

        foreach (var apiCategory in apiCategories.Result)
        {
            var statCategory = GetStatCategory(apiCategory.Entries[0].Id);
            if (apiCategory.Entries.Count == 0 || statCategory == StatCategory.Undefined) continue;

            foreach (var entry in apiCategory.Entries)
            {
                if (invariantStats.IgnoreStatIds.Contains(entry.Id)) continue;
                if (string.IsNullOrEmpty(entry.Id)) continue;

                definitions.AddRange(GetDefinitions(statCategory, entry));
            }
        }

        await dataProvider.Write(game, DataType.TradeStats, language, definitions);
    }

    private StatCategory GetStatCategory(string? apiId)
    {
        var value = apiId?.Split('.').First();
        return value.GetEnumFromValue<StatCategory>();
    }

    private IEnumerable<TradeStatDefinition> GetDefinitions(StatCategory statCategory, RawTradeStat entry)
    {
        List<TradeStatOption>? options = null;
        if (entry.Options?.Options.Count > 0)
        {
            options = [];

            foreach (var option in entry.Options.Options)
            {
                options.Add(new TradeStatOption()
                {
                    Id = option.Id,
                    Text = option.Text.RemoveSquareBrackets(),
                });
            }
        }

        yield return new TradeStatDefinition()
        {
            Category = statCategory,
            Id = entry.Id,
            Text = GetText(entry.Text.RemoveSquareBrackets()),
            Options = options,
        };
    }

    private string GetText(string text, string? optionText = null)
    {
        if (optionText == null) return text;

        var optionLines = new List<string>();
        foreach (var optionLine in newLinePattern.Split(optionText))
        {
            optionLines.Add(parseHashPattern.Replace(text, optionLine));
        }

        return string.Join('\n', optionLines).Trim('\r', '\n');
    }
}
