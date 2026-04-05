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

public class HeistPerceptionProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_perception")?.Text ?? "Perception Level";

    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionHeistPerception.ToRegexHeistLevelCapture();

    public override void Parse(Item item)
    {
        item.Properties.HeistPerceptionLevel = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistPerceptionLevel <= 0) return Task.FromResult<TradeFilter?>(null);
        var filter = new HeistPerceptionFilter
        {
            Text = Label,
            Value = item.Properties.HeistPerceptionLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistPerceptionProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistPerceptionFilter : IntPropertyFilter
{
    public HeistPerceptionFilter() {
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
        query.Filters.GetOrCreateHeistFilters().Filters.Perception = new StatFilterValue(this);
    }
}
