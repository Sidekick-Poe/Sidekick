using System.Text.RegularExpressions;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

/// <summary>
/// "Map Area: Mesa" on Scrying Orbs, Blighted maps and Valdo maps. The trade site has no filter for it.
/// It is parsed before definition parsing, so an item like a Scrying Orb resolves to its area definition ("Scrying Orb (Mesa)").
/// </summary>
public class MapAreaProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex? Pattern { get; } = string.IsNullOrEmpty(gameTextProvider.Texts.ItemPropertyMapArea)
        ? null
        : $"{gameTextProvider.Texts.ItemPropertyMapArea}: #".ToRegexStringProperty();

    public override string Label => gameTextProvider.Texts.ItemPropertyMapArea;

    public override void Parse(Item item)
    {
        if (game != GameType.Poe1 || Pattern == null) return;

        item.Properties.MapArea = GetString(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.Poe1 || string.IsNullOrEmpty(item.Properties.MapArea)) return Task.FromResult<TradeFilter?>(null);

        // Only shown when the item resolved to its area definition. The search for that trade item always targets the area,
        // so the filter is always checked and cannot be changed.
        if (item.Definition.Name?.EndsWith($" ({item.Properties.MapArea})", StringComparison.Ordinal) != true) return Task.FromResult<TradeFilter?>(null);

        var filter = new MapAreaFilter
        {
            Text = Label,
            Value = item.Properties.MapArea,
            Checked = true,
            Readonly = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

/// <summary>
/// Display only: the map area is part of the trade item, so nothing is added to the trade request.
/// </summary>
public class MapAreaFilter : StringPropertyFilter;
