using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.ItemProperties;

public class GemLevelProperty(IGameLanguageProvider gameLanguageProvider)
{
    private Regex? Pattern { get; set; }

    public Task Initialize()
    {
        Pattern = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();
        return Task.CompletedTask;
    }

    public void Parse(Item item, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        item.Properties.GemLevel = GetInt(Pattern, propertyBlock);
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

        if (item.Metadata.Game == GameType.PathOfExile)
        {
            searchFilters.MiscFilters.Filters.GemLevel = filter.Value;
        }
        else if (item.Metadata.Game == GameType.PathOfExile2)
        {
            searchFilters.TypeFilters.Filters.ItemLevel = filter.Value;
        }
    }
}
