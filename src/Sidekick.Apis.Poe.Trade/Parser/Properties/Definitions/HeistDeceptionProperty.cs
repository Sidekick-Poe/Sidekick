using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class HeistDeceptionProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_deception")?.Text ?? "Deception Level";

    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionHeistDeception.ToRegexHeistLevelCapture();

    public override void Parse(Item item)
    {
        if (item.Text.Blocks.Count < 2) return;
        if (game != GameType.PathOfExile1) return;

        item.Properties.HeistDeceptionLevel = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistDeceptionLevel <= 0) return Task.FromResult<TradeFilter?>(null);
        var filter = new HeistDeceptionFilter
        {
            Text = Label,
            Value = item.Properties.HeistDeceptionLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistDeceptionProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistDeceptionFilter : IntPropertyFilter
{
    public HeistDeceptionFilter() {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Default,
            Rules =
            [
                new()
                {
                    Checked = true,
                    Conditions =
                    [
                        new()
                        {
                            Type = AutoSelectConditionType.ItemClass,
                            Comparison = AutoSelectComparisonType.IsContainedIn,
                            Value = JsonSerializer.Serialize(new List<ItemClass>()
                            {
                                ItemClass.HeistContract,
                            }, AutoSelectPreferences.JsonSerializerOptions),
                        },
                    ],
                    NormalizeBy = 0,
                },
            ],
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;
        query.Filters.GetOrCreateHeistFilters().Filters.Deception = new StatFilterValue(this);
    }
}
