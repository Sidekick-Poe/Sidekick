using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class BlightRavagedProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Map];

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.AffixBlightRavaged.ToRegexAffix(gameLanguageProvider.Language.AffixSuperior);
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        itemProperties.BlightRavaged = Pattern?.IsMatch(parsingItem.Blocks[0].Lines[^1].Text) ?? false;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (!item.Properties.BlightRavaged) return null;

        var filter = new BooleanPropertyFilter(this)
        {
            ShowRow = false,
            Text = gameLanguageProvider.Language.AffixBlightRavaged,
            Checked = true,
        };
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked) return;

        searchFilters.GetOrCreateMapFilters().Filters.BlightRavavaged = new SearchFilterOption(filter);
    }
}
