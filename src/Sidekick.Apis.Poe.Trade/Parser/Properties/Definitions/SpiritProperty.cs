using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class SpiritProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionSpirit.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionSpirit.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Weapon, Category.Armour];

    public override void Parse(Item item)
    {
        if(game == GameType.PathOfExile) return;
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Spirit = GetInt(Pattern, propertyBlock);
        if (item.Properties.Spirit == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.Spirit));
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile || item.Properties.Spirit <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionSpirit,
            NormalizeEnabled = true,
            Value = item.Properties.Spirit,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Spirit)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.Spirit = new StatFilterValue(intFilter); break;
        }
    }
}
