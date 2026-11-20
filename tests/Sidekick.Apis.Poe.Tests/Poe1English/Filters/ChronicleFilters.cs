using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Filters.Types;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Filters;

[Collection(Collections.Poe1EnglishFixture)]
public class ChronicleFilters(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public async Task RoomFilters()
    {
        var item = parser.ParseItem(@"Item Class: Misc Map Items
Rarity: Currency
Chronicle of Atzoatl
--------
Area Level: 72
--------
Open Rooms:
Hall of Mettle (Tier 1)
Doryani's Institute (Tier 3)
Jeweller's Workshop (Tier 1)
Toxic Grove (Tier 3)
Chasm
Factory (Tier 3)
Locus of Corruption (Tier 3)
Storage Room (Tier 1)
Pools of Restoration (Tier 1)
Tempest Generator (Tier 1)
Shrine of Unmaking (Tier 3)
Apex of Atzoatl
--------
""The Vaal had little concern for the externalities of their engineering. The toxins produced by their manufacturing may have poisoned the entirety of Atzoatl."" - Icius Perandus, Antiquities Collection, Desiccated Corpse of a Vaal Citizen
--------
Right click this item to view the Temple of Atzoatl.
Can be used in a personal Map Device to open portals to the Temple of Atzoatl in the present day.
");

        var filters = await fixture.StatParser.GetFilters(item);
        var expandableFilter = Assert.IsType<ExpandableFilter>(Assert.Single(filters));
        var statFilters = expandableFilter.Filters.OfType<StatFilter>().ToList();
        Assert.Equal(12, statFilters.Count);
        Assert.Contains(statFilters, x => x.Text == "Doryani's Institute (Tier 3)");
        Assert.Contains(statFilters, x => x.Text == "Apex of Atzoatl");
    }
}
