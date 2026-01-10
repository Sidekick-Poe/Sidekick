using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class MapParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseArcadeMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Arcade Map
--------
Map Tier: 15
Atlas Region: Haewark Hamlet
--------
Item Level: 84
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Arcade Map", actual.ApiInformation.Type);
        Assert.Equal(15, actual.Properties.MapTier);
        Assert.Equal(84, actual.Properties.ItemLevel);
    }

    [Fact]
    public void ParseUniqueMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Unique
Maelström of Chaos
Atoll Map
--------
Map Tier: 5
Atlas Region: Tirn's End
Item Quantity: +41% (augmented)
Item Rarity: +299% (augmented)
--------
Item Level: 73
--------
Area has patches of Chilled Ground
Monsters deal 50% extra Physical Damage as Lightning
Monsters are Immune to randomly chosen Elemental Ailments or Stun
Monsters' Melee Attacks apply random Hexes on Hit
Monsters Reflect Hexes
--------
Whispers from a world apart
Speak my name beyond the tomb;
Bound within the Maelström's heart,
Will they grant me strength or doom?
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Maelström of Chaos", actual.ApiInformation.Name);
        Assert.Equal("Atoll Map", actual.ApiInformation.Type);
        Assert.Equal(5, actual.Properties.MapTier);
        Assert.Equal(41, actual.Properties.ItemQuantity);
        Assert.Equal(299, actual.Properties.ItemRarity);
    }

    [Fact]
    public void ParseOccupiedMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Vortex Crag
Phantasmagoria Map
--------
Map Tier: 12
Atlas Region: Lex Ejoris
Item Quantity: +120% (augmented)
Item Rarity: +61% (augmented)
Monster Pack Size: +39% (augmented)
Quality: +19% (augmented)
--------
Item Level: 79
--------
Area is influenced by The Elder (implicit)
--------
Players are Cursed with Vulnerability
44% more Monster Life
Monsters reflect 18% of Physical Damage
Area contains two Unique Bosses
Monsters take 39% reduced Extra Damage from Critical Strikes
Monsters gain an Endurance Charge on Hit
Monsters cannot be Taunted
Monsters' Action Speed cannot be modified to below base value
Players gain 50% reduced Flask Charges
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
--------
Corrupted
--------
Note: ~price 2 chaos
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Phantasmagoria Map", actual.ApiInformation.Type);

        actual.AssertHasStat(StatCategory.Implicit, "Area is influenced by The Elder");
    }

    [Fact]
    public void ParseTimelessKaruiEmblem()
    {
        var actual = parser.ParseItem(TimelessKaruiEmblem);

        Assert.Equal(ItemClass.MapFragment, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Timeless Karui Emblem", actual.ApiInformation.Type);
    }

    [Fact]
    public void ParseVortexPit()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Vortex Pit
Burial Chambers Map
--------
Map Tier: 16
Atlas Region: Lex Ejoris
Item Quantity: +70% (augmented)
Item Rarity: +31% (augmented)
Monster Pack Size: +20% (augmented)
Quality: +18% (augmented)
--------
Item Level: 82
--------
Area is influenced by The Elder (implicit)
Map is occupied by The Eradicator (implicit)
--------
29% more Magic Monsters
22% more Rare Monsters
Monsters have 100% increased Area of Effect
Monsters Poison on Hit
Rare Monsters each have a Nemesis Mod
Magic Monster Packs each have a Bloodline Mod
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Burial Chambers Map", actual.ApiInformation.Type);

        actual.AssertHasStat(StatCategory.Explicit, "Monsters have #% increased Area of Effect", 100);
    }

    [Fact]
    public void ParseValdoReward()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Cyclopean Monolith
Plaza Map
--------
Map Tier: 17
Reward: Foil Mageblood
Item Quantity: +70% (augmented)
Item Rarity: +28% (augmented)
Monster Pack Size: +22% (augmented)
Quality: +20% (augmented)
--------
Chance for dropped Maps to convert to:
Shaper Map: 18% (augmented)
Elder Map: 9% (augmented)
Conqueror Map: 9% (augmented)
Unique Map: 10% (augmented)
Scarab: 20% (augmented)
--------
Item Level: 100
--------
Monster Level: 84
--------
Rare and Unique Monsters in Area are Possessed by 3 to 4 Tormented Spirits and their Minions are Touched
Monsters have +6 to Maximum Frenzy Charges
Monsters gain a Frenzy Charge on Hit
Area contains The Feared
Players deal 10% less Damage per Equipped Item
Players' Minions deal 10% less Damage per Item Equipped by their Master
Players who Die in area are sent to the Void
Only opens 1 Portal to Area
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once. Defeat 90% of all monsters in this Map, including all Rare and Unique enemies to obtain the Reward. The area created is not affected by your Atlas Passive Tree, and cannot be augmented via the Map Device.
--------
Unmodifiable
--------
Foil (Celestial Amethyst)
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Plaza Map", actual.ApiInformation.Type);
        Assert.Equal("Foil Mageblood", actual.Properties.Reward);
    }

    #region ItemText

    private const string TimelessKaruiEmblem = @"Item Class: Map Fragments
Rarity: Normal
Timeless Karui Emblem
--------
Place two or more different Emblems in a Map Device to access the Domain of Timeless Conflict. Can only be used once.
";

    #endregion ItemText
}
