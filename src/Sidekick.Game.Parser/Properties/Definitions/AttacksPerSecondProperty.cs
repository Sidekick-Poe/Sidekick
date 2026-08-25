using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class AttacksPerSecondProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyAttacksPerSecond.ToRegexDoubleProperty();

    private Regex IsAugmentedPattern { get; } = gameTextProvider.Texts.ItemPropertyAttacksPerSecond.ToRegexIsAugmented();

    public override string Label => gameTextProvider.Texts.ItemPropertyAttacksPerSecond;

    public override void Parse(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique) return;

        item.Properties.AttacksPerSecond = GetDouble(Pattern, item.Text);
        if (item.Properties.AttacksPerSecond == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.AttacksPerSecond));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.AttacksPerSecond <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new AttacksPerSecondFilter(game)
        {
            Text = Label,
            Value = item.Properties.AttacksPerSecond,
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.AttacksPerSecond)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(AttacksPerSecondProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class AttacksPerSecondFilter : DoublePropertyFilter
{
    public AttacksPerSecondFilter(GameType game)
    {
        Game = game;
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    private GameType Game { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (Game)
        {
            case GameType.Poe1: query.Filters.GetOrCreateWeaponFilters().Filters.AttacksPerSecond = new StatFilterValue(this); break;
            case GameType.Poe2: query.Filters.GetOrCreateEquipmentFilters().Filters.AttacksPerSecond = new StatFilterValue(this); break;
        }
    }
}
