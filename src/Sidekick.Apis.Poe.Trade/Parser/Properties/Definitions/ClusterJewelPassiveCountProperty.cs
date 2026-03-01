using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Data.Items;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ClusterJewelPassiveCountProperty(
    GameType game,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly IApiStatsProvider apiStatsProvider = serviceProvider.GetRequiredService<IApiStatsProvider>();

    public override string Label => "Cluster Jewel Passives";

    public override void ParseAfterStats(Item item)
    {
        if (item.Properties.ItemClass != ItemClass.Jewel) return;

        if (game == GameType.PathOfExile2) return;
        if (item.Properties.Rarity == Rarity.Unique) return;

        var passiveCount = ParseSmallPassiveCount(item.Stats);
        if (passiveCount != 0)
        {
            item.Properties.ClusterJewelPassiveCount = passiveCount;
        }

        var grant = ParseGrantTexts(item.Stats);
        if (grant != null) item.Properties.ClusterJewelGrantText = grant;
    }

    private int ParseSmallPassiveCount(List<Stat> lines)
    {
        foreach (var line in lines)
        {
            if (!line.HasValues)
            {
                continue;
            }

            foreach (var definition in line.Definitions)
            {
                if (definition.TradeIds.Contains(apiStatsProvider.InvariantStats.ClusterJewelSmallPassiveCountStatId))
                {
                    return (int)line.AverageValue;
                }
            }
        }

        return 0;
    }

    private string? ParseGrantTexts(List<Stat> lines)
    {
        foreach (var definition in lines.SelectMany(x => x.Definitions))
        {
            if (definition.Option == null) continue;

            if (definition.TradeIds.Contains(apiStatsProvider.InvariantStats.ClusterJewelSmallPassiveGrantStatId))
            {
                return apiStatsProvider.InvariantStats.ClusterJewelSmallPassiveGrantOptions[definition.Option.Id].Replace("\n", ", ");
            }
        }

        return null;
    }
}
