using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class BlockChanceProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyBlockChance.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = gameTextProvider.Texts.ItemPropertyBlockChance.ToRegexIsAugmented();

    public override string Label => gameTextProvider.Texts.ItemPropertyBlockChance;

    public override void Parse(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique) return;

        item.Properties.BlockChance = GetInt(Pattern, item.Text);
        if (item.Properties.BlockChance == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.BlockChance));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.BlockChance <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new BlockChanceFilter(game)
        {
            Text = Label,
            Value = item.Properties.BlockChance,
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.BlockChance)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(BlockChanceProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class BlockChanceFilter : IntPropertyFilter
{
    public BlockChanceFilter(GameType game)
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
            case GameType.PathOfExile1: query.Filters.GetOrCreateArmourFilters().Filters.BlockChance = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.BlockChance = new StatFilterValue(this); break;
        }
    }
}
