using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe1.English;

[Collection(Collections.Poe1EnglishFixture)]
public class FragmentParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

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

        Assert.Equal(ItemClass.MapFragments, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Divine Vessel", actual.TradeItem?.Type);
    }

    [Fact]
    public async Task LocusOfCorruption()
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

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Chronicle of Atzoatl", actual.TradeItem?.Type);
        Assert.Equal(81, actual.Properties.AreaLevel);

        fixture.AssertHasStat(actual, StatCategory.Pseudo, "Has Room: Locus of Corruption (Tier 3)", "Open Room");
        fixture.AssertDoesNotHaveStat(actual, StatCategory.Pseudo, "Has Room: Locus of Corruption (Tier 3)", "Closed Room");

        var filters = await fixture.GetStatFilters(actual);
        Assert.Equal(10, filters.Count);
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

        Assert.Contains(filters, x => x.Text == "Locus of Corruption (Tier 3)");
        Assert.DoesNotContain(filters, x => x.Text == "Apex of Atzoatl");
    }

    [Fact]
    public void VaalAspect()
    {
        var actual = parser.ParseItem(@"Item Class: Pieces
Rarity: Unique
Curiosity
Vaal Aspect
--------
Item Level: 84
--------
--------
Delving into the mysteries of existence, they deciphered
the secret laws of nature... and then, harnessed them.
--------
This item will transform when correctly arranged with other items.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Curiosity", actual.TradeItem?.Name);
        Assert.Equal("Vaal Aspect", actual.TradeItem?.Type);
        Assert.Equal(84, actual.Properties.ItemLevel);
    }

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
