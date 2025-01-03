using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Properties.Definitions;

public class BlockChanceProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex? Pattern { get; set; }

    public override List<Category> ValidCategories { get; } = [Category.Armour];

    public override void Initialize()
    {
        if (game == GameType.PathOfExile)
        {
            Pattern = gameLanguageProvider.Language.DescriptionChanceToBlock.ToRegexIntCapture();
        }
        else
        {
            Pattern = gameLanguageProvider.Language.DescriptionBlockChance.ToRegexIntCapture();
        }
    }

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.BlockChance = GetInt(Pattern, propertyBlock);
        if (itemProperties.BlockChance > 0) propertyBlock.Parsed = true;
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue)
    {
        if (item.Properties.BlockChance <= 0) return null;

        var text = game == GameType.PathOfExile ? gameLanguageProvider.Language.DescriptionChanceToBlock : gameLanguageProvider.Language.DescriptionBlockChance;
        var filter = new IntPropertyFilter(this)
        {
            ShowCheckbox = true,
            Text = text,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.BlockChance,
            Checked = false,
        };
        return filter;
    }

    internal override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        searchFilters.GetOrCreateArmourFilters().Filters.BlockChance = intFilter.Checked ? new StatFilterValue(intFilter) : null;
    }
}
