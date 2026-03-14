using Sidekick.Common.Enums;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using Sidekick.Data.Trade;
using Sidekick.Data.Trade.Raw;
namespace Sidekick.Data.Builder.Trade;

public class TradeStatProvider(DataProvider dataProvider)
{
    public async Task<List<TradeStatDefinition>> GetDefinitions(GameType game, IGameLanguage language)
    {
        var apiCategories = await dataProvider.Read<RawTradeResult<List<RawTradeStatCategory>>>(game, DataType.RawTradeStats, language);
        var invariantStats = await dataProvider.Read<TradeInvariantStats>(game, DataType.StatsInvariant);

        var definitions = new List<TradeStatDefinition>();

        foreach (var apiCategory in apiCategories.Result)
        {
            if (apiCategory.Entries.Count == 0) continue;

            foreach (var entry in apiCategory.Entries)
            {
                if (invariantStats.IgnoreStatIds.Contains(entry.Id)) continue;
                if (string.IsNullOrEmpty(entry.Id)) continue;

                definitions.AddRange(GetDefinitions(entry));
            }
        }

        return definitions;
    }

    private IEnumerable<TradeStatDefinition> GetDefinitions(RawTradeStat entry)
    {
        var category = GetCategory(entry.Id);

        if (entry.Options?.Options.Count > 0)
        {
            foreach (var option in entry.Options.Options)
            {
                yield return new TradeStatDefinition()
                {
                    Id = entry.Id,
                    Category = category,
                    Text = entry.Text.RemoveSquareBrackets(),
                    Option = new TradeStatOption()
                    {
                        Id = option.Id,
                        Text = option.Text.RemoveSquareBrackets(),
                    },
                };
            }
        }
        else
        {
            yield return new TradeStatDefinition()
            {
                Id = entry.Id,
                Category = category,
                Text = entry.Text.RemoveSquareBrackets(),
            };
        }

        yield break;

        StatCategory GetCategory(string id)
        {
            var value = id.Split('.').First();
            return value.GetEnumFromValue<StatCategory>();
        }
    }
}
