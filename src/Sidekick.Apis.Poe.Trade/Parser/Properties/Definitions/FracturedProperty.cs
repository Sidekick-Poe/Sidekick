using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class FracturedProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.WithStats,
    ];

    public override string Label => tradeFilterProvider.Fractured?.Text ?? "Fractured";

    public override void Parse(Item item)
    {
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (tradeFilterProvider.Fractured == null) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(FracturedProperty)}_{game.GetValueAttribute()}";
        var filter = new FracturedFilter
        {
            Text = Label,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class FracturedFilter : TriStatePropertyFilter
{
    public FracturedFilter()
    {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Any,
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Fractured = new SearchFilterOption(this);
    }
}
