using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public class PseudoParser
(
    IInvariantStatsProvider invariantStatsProvider,
    IApiStatsProvider apiStatsProvider,
    ISettingsService settingsService,
    IStringLocalizer<PoeResources> resources
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

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        if (!ItemClassConstants.WithStats.Contains(item.Properties.ItemClass)) return [];

        var result = new List<TradeFilter>();
        foreach (var stat in item.PseudoStats)
        {
            var autoSelectKey = $"Trade_Filter_Pseudo_{item.Game.GetValueAttribute()}_{stat.Id}";
            var autoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null);

            var filter = new PseudoFilter(stat)
            {
                AutoSelectSettingKey = autoSelectKey,
                AutoSelect = autoSelect,
            };

            result.Add(filter);
            filter.Initialize(item);
        }

        return
        [
            new ExpandableFilter(resources["Pseudo_Filters"], result.ToArray())
            {
                Checked = true,
            },
        ];
    }
}
