using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class ArmourProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionArmour.ToRegexIntCapture();

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.Armour = GetInt(Pattern, propertyBlock);
        if (itemProperties.Armour > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.Armour <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionArmour,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.ArmourWithQuality,
            OriginalValue = item.Properties.Armour,
            Checked = false,
        };
        filter.NormalizeMinValue();
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: searchFilters.GetOrCreateArmourFilters().Filters.Armour = new StatFilterValue(intFilter); break;
            case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.Armour = new StatFilterValue(intFilter); break;
        }
    }
}
