using Sidekick.Common.Enums;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class ItemClassProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    public override string Label => gameTextProvider.Texts.ItemPropertyItemClass;

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Rarity is not (Rarity.Rare or Rarity.Magic or Rarity.Normal)) return Task.FromResult<TradeFilter?>(null);
        if (item.ItemClass.Type == ItemClass.Unknown) return Task.FromResult<TradeFilter?>(null);

        var classLabel = item.ItemClass.Name;
        if (classLabel == null || item.TradeItem?.Type == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new ItemClassFilter
        {
            Text = gameTextProvider.Texts.ItemPropertyItemClass,
            ItemClass = classLabel,
            BaseTypeText = gameTextProvider.Texts.TradeBaseType,
            BaseType = item.TradeItem.Type,
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
