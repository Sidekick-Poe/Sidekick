using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class EnergyShieldProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game
) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    private Regex? AlternatePattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionEnergyShield.ToRegexIntCapture();
        if (!string.IsNullOrEmpty(gameLanguageProvider.Language.DescriptionEnergyShieldAlternate))
        {
            AlternatePattern = gameLanguageProvider.Language.DescriptionEnergyShieldAlternate.ToRegexIntCapture();
        }
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.EnergyShield = GetInt(Pattern, propertyBlock);
        if (itemProperties.EnergyShield <= 0 && AlternatePattern != null) itemProperties.EnergyShield = GetInt(AlternatePattern, propertyBlock);
        if (itemProperties.EnergyShield > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.EnergyShield <= 0) return null;

        var text = gameLanguageProvider.Language.DescriptionEnergyShield;
        if (!string.IsNullOrEmpty(gameLanguageProvider.Language.DescriptionEnergyShieldAlternate) && item.Header.Game == GameType.PathOfExile2)
        {
            text = gameLanguageProvider.Language.DescriptionEnergyShieldAlternate;
        }

        var filter = new IntPropertyFilter(this)
        {
            Text = text,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.EnergyShieldWithQuality,
            OriginalValue = item.Properties.EnergyShield,
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
            case GameType.PathOfExile: searchFilters.GetOrCreateArmourFilters().Filters.EnergyShield = new StatFilterValue(intFilter); break;
            case GameType.PathOfExile2: searchFilters.GetOrCreateEquipmentFilters().Filters.EnergyShield = new StatFilterValue(intFilter); break;
        }
    }
}
