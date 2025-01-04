using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class AttacksPerSecondProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Weapon];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionAttacksPerSecond.ToRegexDoubleCapture();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.AttacksPerSecond = GetDouble(Pattern, propertyBlock);
        if (itemProperties.AttacksPerSecond > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.AttacksPerSecond <= 0) return null;

        var filter = new DoublePropertyFilter(this)
        {
            ShowCheckbox = true,
            Text = gameLanguageProvider.Language.DescriptionAttacksPerSecond,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.AttacksPerSecond,
            Checked = false,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not DoublePropertyFilter doubleFilter) return;

        searchFilters.GetOrCreateWeaponFilters().Filters.AttacksPerSecond = doubleFilter.Checked ? new StatFilterValue(doubleFilter) : null;
    }
}
