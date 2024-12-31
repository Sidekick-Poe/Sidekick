using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class GemLevelProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override bool Enabled => true;

    public override void Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();
    }

    public override void ParseBeforeModifiers(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        if (parsingItem.Header?.Category != Category.Gem) return;

        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.GemLevel = GetInt(Pattern, propertyBlock);
    }

    public override void ParseAfterModifiers(ItemProperties itemProperties, ParsingItem parsingItem, List<ModifierLine> modifierLines)
    {
    }

    public PropertyFilter? GetFilter(Item item)
    {
        if (item.Properties.GemLevel <= 0)
        {
            return null;
        }

        var filter = new PropertyFilter(true, PropertyFilterType.Misc_GemLevel, gameLanguageProvider.Language.DescriptionLevel, item.Properties.GemLevel, null);
        return filter;
    }

    public void SetTradeRequest(SearchFilters searchFilters, PropertyFilter filter, Item item)
    {
        if (filter.Type != PropertyFilterType.Misc_GemLevel || filter.Checked != true)
        {
            return;
        }

        if (item.Header.Game == GameType.PathOfExile)
        {
            searchFilters.GetOrCreateMiscFilters().Filters.GemLevel = new StatFilterValue()
            {
                Min = filter.Value,

            }
        }
        else if (item.Header.Game == GameType.PathOfExile2)
        {
            searchFilters.TypeFilters.Filters.ItemLevel = filter.Value;
        }
    }
}
