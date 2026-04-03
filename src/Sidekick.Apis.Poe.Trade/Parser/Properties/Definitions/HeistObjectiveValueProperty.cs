using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class HeistObjectiveValueProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_objective_value")?.Text ?? "Contract Objective Value";

    public override void Parse(Item item)
    {
        if (item.Text.Blocks.Count < 2) return;
        if (game != GameType.PathOfExile1) return;
        if (item.ItemClass != ItemClass.HeistContract) return;

        foreach (var line in item.Text.Blocks[1].Lines)
        {
            if (line.Text.Contains(currentGameLanguage.Language.DescriptionHeistModerateValue))
            {
                item.Properties.HeistObjectiveValue = HeistObjectiveValue.Moderate;
                return;
            }
            if (line.Text.Contains(currentGameLanguage.Language.DescriptionHeistHighValue))
            {
                item.Properties.HeistObjectiveValue = HeistObjectiveValue.High;
                return;
            }
            if (line.Text.Contains(currentGameLanguage.Language.DescriptionHeistPriceless))
            {
                item.Properties.HeistObjectiveValue = HeistObjectiveValue.Priceless;
                return;
            }
            if (line.Text.Contains(currentGameLanguage.Language.DescriptionHeistPrecious))
            {
                item.Properties.HeistObjectiveValue = HeistObjectiveValue.Precious;
                return;
            }
        }
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistObjectiveValue == HeistObjectiveValue.Undefined) return Task.FromResult<TradeFilter?>(null);

        var filter = new HeistObjectiveValueFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistObjectiveValueProperty)}_{game.GetValueAttribute()}",
            Value = item.Properties.HeistObjectiveValue.GetValueAttribute(),
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistObjectiveValueFilter : StringPropertyFilter
{
    public HeistObjectiveValueFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked || Value == HeistObjectiveValue.Undefined.GetValueAttribute()) return;
        query.Filters.GetOrCreateHeistFilters().Filters.ObjectiveValue = new SearchFilterOption(Value);
    }
}
