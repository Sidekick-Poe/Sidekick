using Sidekick.Apis.Poe.Models;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class IncursionParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

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

        Assert.Equal(ItemClass.MapFragment, actual.Header.ItemClass);
        Assert.Equal(Category.Map, actual.Header.Category);
        Assert.Equal(Rarity.Currency, actual.Header.Rarity);
        Assert.Equal("Chronicle of Atzoatl", actual.Header.ApiType);
        Assert.Equal(81, actual.Properties.AreaLevel);

        actual.AssertHasModifier(ModifierCategory.Pseudo, "Has Room: Locus of Corruption (Tier 3)");
    }
}
