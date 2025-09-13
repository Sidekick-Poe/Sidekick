using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Models;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class BlockChanceProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = game is GameType.PathOfExile
        ? gameLanguageProvider.Language.DescriptionChanceToBlock.ToRegexIntCapture()
        : gameLanguageProvider.Language.DescriptionBlockChance.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = game is GameType.PathOfExile
        ? gameLanguageProvider.Language.DescriptionChanceToBlock.ToRegexIsAugmented()
        : gameLanguageProvider.Language.DescriptionBlockChance.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.BlockChance = GetInt(Pattern, propertyBlock);
        if (itemProperties.BlockChance == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) itemProperties.AugmentedProperties.Add(nameof(ItemProperties.BlockChance));
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.BlockChance <= 0) return null;

        var text = game == GameType.PathOfExile ? gameLanguageProvider.Language.DescriptionChanceToBlock : gameLanguageProvider.Language.DescriptionBlockChance;
        var filter = new IntPropertyFilter(this)
        {
            Text = text,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.BlockChance,
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.BlockChance)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: query.Filters.GetOrCreateArmourFilters().Filters.BlockChance = new StatFilterValue(intFilter); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.BlockChance = new StatFilterValue(intFilter); break;
        }
    }
}
