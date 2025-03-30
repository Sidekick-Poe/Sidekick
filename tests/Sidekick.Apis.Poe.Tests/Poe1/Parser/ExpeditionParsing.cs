using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class ExpeditionParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseMagicLogbook()
    {
        var actual = parser.ParseItem(@"Item Class: Expedition Logbooks
Rarity: Magic
Chaining Expedition Logbook
--------
Item Quantity: +16% (augmented)
Item Rarity: +9% (augmented)
Monster Pack Size: +6% (augmented)
Area Level: 69
--------
Item Level: 69
--------
Rotting Temple
Druids of the Broken Circle
32% increased quantity of Artifacts dropped by Monsters (implicit)
35% increased Explosive Radius (implicit)
Area contains an additional Underground Area (implicit)
--------
Sarn Slums
Druids of the Broken Circle
29% increased Explosive Radius (implicit)
Area contains 17% increased number of Monster Markers (implicit)
Area contains 36% increased number of Remnants (implicit)
--------
Scrublands
Order of the Chalice
Area contains 8 additional Chest Markers (implicit)
31% increased Explosive Placement Range (implicit)
Area contains 30% increased number of Monster Markers (implicit)
--------
Monsters' skills Chain 2 additional times
--------
Take this item to Dannig in your Hideout to open portals to an expedition.
");

        Assert.Equal("logbook", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Equal(Category.Logbook, actual.Header.Category);
        Assert.Equal("Expedition Logbook", actual.Header.ApiType);

        actual.AssertHasModifier(ModifierCategory.Pseudo, "Has Logbook Faction: Druids of the Broken Circle");
        actual.AssertHasModifier(ModifierCategory.Pseudo, "Has Logbook Faction: Order of the Chalice");
    }

    [Fact]
    public void ParseRareLogbook()
    {
        var actual = parser.ParseItem(@"Item Class: Expedition Logbooks
Rarity: Rare
Bleak Myth
Expedition Logbook
--------
Item Quantity: +80% (augmented)
Item Rarity: +46% (augmented)
Monster Pack Size: +30% (augmented)
Area Level: 83
--------
Item Level: 83
--------
Battleground Graves
Druids of the Broken Circle
35% increased Explosive Radius (implicit)
32% increased number of Explosives (implicit)
22% increased Explosive Placement Range (implicit)
--------
Scrublands
Druids of the Broken Circle
11% increased quantity of Artifacts dropped by Monsters (implicit)
13% increased Explosive Radius (implicit)
Area contains an additional Underground Area (implicit)
--------
Players have -12% to all maximum Resistances
Monsters have 396% increased Critical Strike Chance
+44% to Monster Critical Strike Multiplier
25% increased Monster Damage
Monsters deal 107% extra Physical Damage as Fire
Monsters Hinder on Hit with Spells
--------
Take this item to Dannig in your Hideout to open portals to an expedition.
");

        Assert.Equal("logbook", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal(Category.Logbook, actual.Header.Category);
        Assert.Equal("Expedition Logbook", actual.Header.ApiType);

        actual.AssertHasModifier(ModifierCategory.Pseudo, "Has Logbook Faction: Druids of the Broken Circle");
    }
}
