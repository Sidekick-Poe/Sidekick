using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class EnergyShieldProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionEnergyShield.ToRegexIntCapture();
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.EnergyShield = GetInt(Pattern, propertyBlock);
        if (itemProperties.EnergyShield > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.EnergyShield <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            ShowCheckbox = true,
            Text = gameLanguageProvider.Language.DescriptionEnergyShield,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.EnergyShield,
            Checked = false,
        };
        return filter;
    }

    internal override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateArmourFilters().Filters.EnergyShield = intFilter.Checked ? new StatFilterValue(intFilter) : null;
    }
}
