using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlockChanceProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = game is GameType.PathOfExile1
        ? gameLanguageProvider.Language.DescriptionChanceToBlock.ToRegexIntCapture()
        : gameLanguageProvider.Language.DescriptionBlockChance.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = game is GameType.PathOfExile1
        ? gameLanguageProvider.Language.DescriptionChanceToBlock.ToRegexIsAugmented()
        : gameLanguageProvider.Language.DescriptionBlockChance.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
    ];

    public override string Label => game == GameType.PathOfExile1 ? gameLanguageProvider.Language.DescriptionChanceToBlock : gameLanguageProvider.Language.DescriptionBlockChance;

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.BlockChance = GetInt(Pattern, propertyBlock);
        if (item.Properties.BlockChance == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.BlockChance));
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.BlockChance <= 0) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(BlockChanceProperty)}_{game.GetValueAttribute()}";
        var filter = new BlockChanceFilter(game)
        {
            Text = Label,
            NormalizeEnabled = true,
            Value = item.Properties.BlockChance,
            ValueSuffix = "%",
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.BlockChance)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class BlockChanceFilter : IntPropertyFilter
{
    public BlockChanceFilter(GameType game)
    {
        Game = game;
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Never,
        };
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
