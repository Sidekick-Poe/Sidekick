using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ArmourProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionArmour.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionArmour.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Armour = GetInt(Pattern, propertyBlock);
        if (item.Properties.Armour == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.Armour));
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (item.Properties.Armour <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionArmour,
            NormalizeEnabled = true,
            Value = item.Properties.ArmourWithQuality,
            OriginalValue = item.Properties.Armour,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Armour)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateArmourFilters().Filters.Armour = new StatFilterValue(intFilter); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.Armour = new StatFilterValue(intFilter); break;
        }
    }
}
