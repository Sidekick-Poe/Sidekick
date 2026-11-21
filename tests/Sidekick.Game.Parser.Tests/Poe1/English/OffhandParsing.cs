using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe1.English;

[Collection(Collections.Poe1EnglishFixture)]
public class OffhandParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void GloomSpell()
    {
        var actual = parser.ParseItem(@"Item Class: Shields
Rarity: Rare
Gloom Spell
Titanium Spirit Shield
--------
Quality: +20% (augmented)
Chance to Block: 43% (augmented)
Energy Shield: 226 (augmented)
--------
Requirements:
Level: 68
Int: 159
--------
Sockets: B-B-B 
--------
Item Level: 86
--------
{ Prefix Modifier ""Seraphim's"" (Tier: 1) � Defences, Energy Shield }
42(39-42)% increased Energy Shield
17(16-17)% increased Stun and Block Recovery
{ Prefix Modifier ""Enduring"" (Tier: 2) }
75(70-75)% increased Chance to Block
{ Prefix Modifier ""Unfaltering"" (Tier: 1) � Defences, Energy Shield }
109(101-110)% increased Energy Shield
{ Suffix Modifier ""of Steel Skin"" (Tier: 3) }
22(20-22)% increased Stun and Block Recovery
");

        Assert.Equal(ItemClass.Shield, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Titanium Spirit Shield", actual.TradeItem?.Type);

        Assert.Equal(86, actual.Properties.ItemLevel);
        Assert.False(actual.Properties.Unidentified);
        Assert.False(actual.Properties.Corrupted);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Energy Shield (Local)", 151);
    }

    [Fact]
    public void FoulbornSquire()
    {
        var actual = parser.ParseItem(@"Item Class: Shields
Rarity: Unique
Foulborn The Squire
Elegant Round Shield
--------
Chance to Block: 29% (augmented)
Armour: 345 (augmented)
Evasion Rating: 345 (augmented)
--------
Requirements:
Level: 70
Str: 85
Dex: 85
--------
Sockets: W-W-W 
--------
Item Level: 83
--------
120% increased Block Recovery (implicit)
--------
Has 3 Sockets
Socketed Support Gems can also Support Skills from your Main Hand
113% increased Armour and Evasion
+4% Chance to Block
+1 to Level of Socketed Gems (mutated)
Ignore Attribute Requirements of Socketed Gems (mutated)
--------
Judge not the weak, for
they empower the strong.
--------
Note: ~b/o 280 divine
");

        Assert.Equal(ItemClass.Shield, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("The Squire", actual.TradeItem?.Name);
        Assert.Equal("Elegant Round Shield", actual.TradeItem?.Type);
        Assert.True(actual.Properties.Foulborn);
        actual.AssertHasNinjaStash("foulborn-the-squire-gem-level-no-requirements-elegant-round-shield");
    }

    [Fact]
    public void FoulbornRathpithGlobe()
    {
        var actual = parser.ParseItem(@"Item Class: Shields
Rarity: Unique
Foulborn Rathpith Globe
Titanium Spirit Shield
--------
Chance to Block: 25%
Energy Shield: 167 (augmented)
--------
Requirements:
Level: 68
Int: 159
--------
Sockets: R-R-G 
--------
Item Level: 85
--------
14% Chance to Block Spell Damage
122% increased Energy Shield
10% increased maximum Life
Sacrifice 10% of your Life when you Use or Trigger a Spell Skill
5% increased Spell Damage per 100 Player Maximum Life
Deal 5% increased Damage Over Time per 100 Player Maximum Life (mutated)
--------
The Vaal emptied their slaves of beating hearts,
and left a mountain of twitching dead.
--------
Note: ~b/o 250 divine
");

        Assert.Equal(ItemClass.Shield, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Rathpith Globe", actual.TradeItem?.Name);
        Assert.Equal("Titanium Spirit Shield", actual.TradeItem?.Type);
        Assert.True(actual.Properties.Foulborn);
        actual.AssertHasNinjaStash("foulborn-rathpith-globe-dot-per-life-titanium-spirit-shield");
    }
}
