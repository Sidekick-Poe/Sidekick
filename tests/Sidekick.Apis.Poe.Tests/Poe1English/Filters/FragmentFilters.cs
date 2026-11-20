using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Properties.Definitions;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Filters;

[Collection(Collections.Poe1EnglishFixture)]
public class FragmentFilters(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public async Task TempleOfAtzoatl()
    {
        var item = parser.ParseItem(@"Item Class: Misc Map Items
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

        var filters = await fixture.GetStatFilters(item);
        Assert.Equal(12, filters.Count);
        var index = -1;
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
    }
}
