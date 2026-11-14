using System.Text.RegularExpressions;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

/// <summary>
/// "Map Area: Mesa" on Scrying Orbs, Blighted maps and Valdo maps. There is no trade filter for it;
/// the value is used to resolve the area-specific trade item (e.g. "Scrying Orb (Mesa)").
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
}
