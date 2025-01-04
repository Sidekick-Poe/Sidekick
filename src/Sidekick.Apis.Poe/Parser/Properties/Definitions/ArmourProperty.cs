using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class ArmourProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionArmour.ToRegexIntCapture();
    }

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
            ShowCheckbox = true,
            Text = gameLanguageProvider.Language.DescriptionArmour,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.Armour,
            Checked = false,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateArmourFilters().Filters.Armour = intFilter.Checked ? new StatFilterValue(intFilter) : null;
    }
}
