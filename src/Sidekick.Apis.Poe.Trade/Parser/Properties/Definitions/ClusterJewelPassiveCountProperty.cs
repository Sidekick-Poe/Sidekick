using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ClusterJewelPassiveCountProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly IInvariantStatsProvider invariantStatsProvider = serviceProvider.GetRequiredService<IInvariantStatsProvider>();

    public override List<ItemClass> ValidItemClasses { get; } = [ItemClass.Jewel];

    public override void ParseAfterStats(Item item)
    {
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

            foreach (var stat in line.ApiInformation)
            {
                if (stat.Id == invariantStatsProvider.ClusterJewelSmallPassiveCountStatId)
                {
                    return (int)line.AverageValue;
                }
            }
        }

        return 0;
    }

    private string? ParseGrantTexts(List<Stat> lines)
    {
        foreach (var line in lines)
        {
            if (!line.OptionValue.HasValue)
            {
                continue;
            }

            foreach (var apiStat in line.ApiInformation)
            {
                if (apiStat.Id == invariantStatsProvider.ClusterJewelSmallPassiveGrantStatId)
                {
                    return invariantStatsProvider.ClusterJewelSmallPassiveGrantOptions[line.OptionValue.Value].Replace("\n", ", ");
                }
            }
        }

        return null;
    }
}
