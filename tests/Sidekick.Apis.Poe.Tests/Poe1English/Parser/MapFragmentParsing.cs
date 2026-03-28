using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class MapFragmentParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void DivineVessel()
    {
        var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Divine Vessel
--------
Unique Boss deals 10% increased Damage
Unique Boss has 10% increased Attack and Cast Speed
Unique Boss has 10% increased Life
Unique Boss has 20% increased Area of Effect
--------
Power is a curious thing. 
It can be contained, hidden, locked away, 
and yet it always breaks free.
--------
Can be used in a personal Map Device, allowing you to capture the Soul of the Map's Boss. The Vessel containing the captured Soul can be retrieved from the Map Device. You must be in the Map when the boss is defeated.
--------
Note: ~price 1 chaos
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Divine Vessel", actual.Definition.TradeItem?.Type);
    }

    [Fact]
    public void LocusOfCorruption()
    {
        var actual = parser.ParseItem(@"Item Class: Misc Map Items
Rarity: Currency
Chronicle of Atzoatl
--------
Area Level: 81
--------
Open Rooms:
Sparring Room (Tier 1)
Glittering Halls (Tier 3)
Locus of Corruption (Tier 3)
Demolition Lab (Tier 2)
Temple Nexus (Tier 3)
Apex of Ascension (Tier 3)
Tempest Generator (Tier 1)
Automaton Lab (Tier 2)
Cultivar Chamber (Tier 2)
Pools of Restoration (Tier 1)
Obstructed Rooms:
Banquet Hall
Apex of Atzoatl
--------
""Atzoatl was a locus of Corruption, a temple dedicated to the worship of the unspeakable."" - Icius Perandus, Antiquities Collection, Eroded Vaal Orb
--------
Can be used in a personal Map Device to open portals to the Temple of Atzoatl in the present day.
--------
Note: ~price 1.29 exalted
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Chronicle of Atzoatl", actual.Definition.TradeItem?.Type);
        Assert.Equal(81, actual.Properties.AreaLevel);

        fixture.AssertHasStat(actual, StatCategory.Pseudo, "Has Room: Locus of Corruption (Tier 3)", "Open Room");
    }
}
