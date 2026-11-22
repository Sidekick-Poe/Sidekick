using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe2.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class ArmourParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void PainGuardian()
    {
        var actual = parser.ParseItem(@"Item Class: Body Armours
Rarity: Rare
Pain Guardian
Sacramental Robe
--------
Quality: +20% (augmented)
Energy Shield: 600 (augmented)
Runic Ward: 30 (augmented)
--------
Requires: Level 75, 84 (augmented) Int
--------
Sockets: S S 
--------
Item Level: 79
--------
+25 to maximum Runic Ward (rune)
18% increased Armour, Evasion and Energy Shield (rune)
--------
{ Implicit Modifier — Energy Shield }
20(20-25)% increased Energy Shield Recharge Rate
--------
{ Desecrated Prefix Modifier ""Pope's"" (Tier: 1) — Life, Energy Shield }
42(39-42)% increased Energy Shield
+46(42-49) to maximum Life
{ Prefix Modifier ""Archon's"" (Tier: 2) — Energy Shield }
+24(21-25) to maximum Energy Shield
37(33-38)% increased Energy Shield
{ Prefix Modifier ""Scintillating"" (Tier: 3) — Energy Shield }
+77(74-80) to maximum Energy Shield
{ Suffix Modifier ""of the Starfish"" (Tier: 8) — Life }
4.7(4.1-6) Life Regeneration per second
{ Suffix Modifier ""of the Skilled"" (Tier: 2) }
30% reduced Attribute Requirements
{ Crafted Suffix Modifier ""of the Essence"" }
Hits against you have 43(40-50)% reduced Critical Damage Bonus
");

        Assert.Equal(ItemClass.BodyArmour, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Sacramental Robe", actual.TradeItem?.Type);

        Assert.Equal(79, actual.Properties.ItemLevel);
        Assert.Equal(600, actual.Properties.EnergyShield);
        Assert.Equal(30, actual.Properties.RunicWard);

        Assert.Equal(75, actual.Properties.RequiresLevel);
        Assert.Equal(84, actual.Properties.RequiresIntelligence);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "#% increased Energy Shield Recharge Rate", 20);
        fixture.AssertHasStat(actual, StatCategory.Desecrated, "#% increased Energy Shield", 42);
        fixture.AssertHasStat(actual, StatCategory.Desecrated, "# to maximum Life", 46);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "# to maximum Energy Shield", 101);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Energy Shield", 37);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "# Life Regeneration per second", 4.7);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Attribute Requirements", -30);
        fixture.AssertHasStat(actual, StatCategory.Crafted, "Hits against you have #% reduced Critical Damage Bonus", 43);
    }
}
