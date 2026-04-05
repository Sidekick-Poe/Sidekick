using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using ItemProperties = Sidekick.Data.Items.ItemProperties;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlockChanceProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = game is GameType.PathOfExile1
        ? currentGameLanguage.Language.DescriptionChanceToBlock.ToRegexIntProperty()
        : currentGameLanguage.Language.DescriptionBlockChance.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = game is GameType.PathOfExile1
        ? currentGameLanguage.Language.DescriptionChanceToBlock.ToRegexIsAugmented()
        : currentGameLanguage.Language.DescriptionBlockChance.ToRegexIsAugmented();

    public override string Label => game == GameType.PathOfExile1 ? currentGameLanguage.Language.DescriptionChanceToBlock : currentGameLanguage.Language.DescriptionBlockChance;

    public override void Parse(Item item)
    {
        if (!item.ItemClass.IsEquipment()) return;

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
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.BlockChance)) ? LineContentType.Augmented : LineContentType.Simple,
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
