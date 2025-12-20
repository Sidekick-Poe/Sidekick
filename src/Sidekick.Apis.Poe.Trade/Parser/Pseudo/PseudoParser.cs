using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public class PseudoParser
(
    IInvariantStatsProvider invariantStatsProvider,
    IApiStatsProvider apiStatsProvider,
    ISettingsService settingsService
) : IPseudoParser
{
    private List<PseudoDefinition> Definitions { get; } = new();

    /// <inheritdoc/>
    public int Priority => 300;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();

        Definitions.Clear();
        Definitions.AddRange([
            new ElementalResistancesDefinition(),
            new ChaosResistancesDefinition(),
            new StrengthDefinition(),
            new IntelligenceDefinition(),
            new DexterityDefinition(),
            new LifeDefinition(game),
            new ManaDefinition(game),
        ]);

        var categories = await invariantStatsProvider.GetList();
        categories.RemoveAll(x => x.Entries.FirstOrDefault()?.Id.StartsWith("pseudo") == true);

        var localizedPseudoStats = apiStatsProvider.Definitions.GetValueOrDefault(StatCategory.Pseudo);

        foreach (var definition in Definitions)
        {
            definition.InitializeDefinition(categories, localizedPseudoStats);
        }
    }

    public void Parse(Item item)
    {
        item.PseudoStats.Clear();
        foreach (var definition in Definitions)
        {
            var result = definition.Parse(item.Stats);
            if (result != null && !string.IsNullOrEmpty(result.Text)) item.PseudoStats.Add(result);
        }
    }

    public List<PseudoFilter> GetFilters(Item item)
    {
        if (!ItemClassConstants.WithStats.Contains(item.Properties.ItemClass)) return [];

        var result = new List<PseudoFilter>();
        foreach (var stat in item.PseudoStats)
        {
            result.Add(new PseudoFilter(stat));
        }

        return result;
    }
}
