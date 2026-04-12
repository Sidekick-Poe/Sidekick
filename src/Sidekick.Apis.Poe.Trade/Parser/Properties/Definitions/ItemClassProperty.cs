using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemClassProperty(
    GameType game,
    IStringLocalizer<PoeResources> resources) : PropertyDefinition
{
    public override string Label => resources["Item_Class"];

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Rarity is not (Rarity.Rare or Rarity.Magic or Rarity.Normal)) return Task.FromResult<TradeFilter?>(null);
        if (item.ItemClass.Type == ItemClass.Unknown) return Task.FromResult<TradeFilter?>(null);

        var classLabel = item.ItemClass.Name;
        if (classLabel == null || item.Definition.TradeItem?.Type == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new ItemClassFilter
        {
            Text = resources["Item_Class"],
            ItemClass = classLabel,
            BaseTypeText = resources["Base_Type"],
            BaseType = item.Definition.TradeItem.Type,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ItemClassProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class ItemClassFilter : TradeFilter
{
    public ItemClassFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public required string ItemClass { get; init; }
    public required string BaseType { get; init; }
    public required string BaseTypeText { get; init; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Type = null;
        query.Filters.GetOrCreateTypeFilters().Filters.Category = GetCategoryFilter(item);
    }

    private static SearchFilterOption? GetCategoryFilter(Item item)
    {
        var id = item.ItemClass.Type.FindAttribute<ItemClassTradeId>(attr => attr.Game == item.Game)?.Id;
        if (string.IsNullOrEmpty(id)) return null;

        return new SearchFilterOption(id);
    }
}
