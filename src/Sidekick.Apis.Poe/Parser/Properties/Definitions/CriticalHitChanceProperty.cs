using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class CriticalHitChanceProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Weapon];

    public override void Initialize()
    {
        if (game == GameType.PathOfExile)
        {
            Pattern = gameLanguageProvider.Language.DescriptionCriticalStrikeChance.ToRegexDoubleCapture();
        }
        else
        {
            Pattern = gameLanguageProvider.Language.DescriptionCriticalHitChance.ToRegexDoubleCapture();
        }
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.CriticalHitChance = GetDouble(Pattern, propertyBlock);
        if (itemProperties.CriticalHitChance > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.CriticalHitChance <= 0) return null;

        var text = game == GameType.PathOfExile ? gameLanguageProvider.Language.DescriptionCriticalStrikeChance : gameLanguageProvider.Language.DescriptionCriticalHitChance;
        var filter = new DoublePropertyFilter(this)
        {
            ShowCheckbox = true,
            Text = text,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.CriticalHitChance,
            Checked = false,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not DoublePropertyFilter doubleFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: searchFilters.GetOrCreateWeaponFilters().Filters.CriticalHitChance = new StatFilterValue(doubleFilter); break;
            case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.CriticalHitChance = new StatFilterValue(doubleFilter); break;
        }
    }
}
