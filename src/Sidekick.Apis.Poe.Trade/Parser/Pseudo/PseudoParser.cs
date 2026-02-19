using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.ApiStats;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
using Sidekick.Data.Trade;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public class PseudoParser
(
    IApiStatsProvider apiStatsProvider,
    ISettingsService settingsService,
    IStringLocalizer<PoeResources> resources,
    TradeDataProvider tradeDataProvider,
    IGameLanguageProvider gameLanguageProvider
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

        var categories = await tradeDataProvider.GetRawStats(game, gameLanguageProvider.InvariantLanguage.Code);
        categories.RemoveAll(x => x.Entries.FirstOrDefault()?.Id.StartsWith("pseudo") == true);

        var pseudoDefinitions = apiStatsProvider.Definitions
            .Where(x=>x.Category == StatCategory.Pseudo)
            .ToList();

        foreach (var definition in Definitions)
        {
            definition.InitializeDefinition(categories, pseudoDefinitions);
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
            result.Add(new PseudoFilter(stat)
            {
                AutoSelectSettingKey = $"Trade_Filter_Pseudo_{item.Game.GetValueAttribute()}_{stat.Id}",
            });
        }

        var expandableFilter = new ExpandableFilter(resources["Pseudo_Filters"], result.ToArray())
        {
            Checked = true,
        };
        await expandableFilter.Initialize(item, settingsService);
        expandableFilter.Checked = true;

        return [expandableFilter];
    }
}
