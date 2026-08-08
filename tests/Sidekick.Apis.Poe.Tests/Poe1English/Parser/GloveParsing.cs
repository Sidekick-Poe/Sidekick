using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class GloveParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseRareGloves()
    {
        var actual = parser.ParseItem(@"Item Class: Gloves
Rarity: Rare
Death Nails
Assassin's Mitts
--------
Evasion Rating: 104
Energy Shield: 20
--------
Requirements:
Level: 58
Dex: 45
Int: 45
--------
Sockets: G
--------
Item Level: 61
--------
+18 to Intelligence
+73 to maximum Life
+14% to Lightning Resistance
0.23% of Physical Attack Damage Leeched as Mana
");

        Assert.Equal(ItemClass.Gloves, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Assassin's Mitts", actual.TradeItem?.Type);
        Assert.Equal("Death Nails", actual.Name);
        Assert.NotNull(actual.Properties.Sockets);
        Assert.Single(actual.Properties.Sockets);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "+# to Intelligence", 18);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "+# to maximum Life", 73);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "+#% to Lightning Resistance", 14);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% of Physical Attack Damage Leeched as Mana", 0.23);
    }

    [Fact]
    public void ParseCurseOnHit()
    {
        var actual = parser.ParseItem(@"Item Class: Gloves
Rarity: Unique
Asenath's Gentle Touch
Silk Gloves
--------
Quality: +20% (augmented)
Energy Shield: 28 (augmented)
--------
Requirements:
Level: 25
Int: 39
--------
Sockets: B 
--------
Item Level: 80
--------
Curse Enemies with Despair on Hit (implicit)
Curse Enemies with Enfeeble on Hit (implicit)
--------
+25 to Intelligence
+67 to maximum Life
+72 to maximum Mana
Curse Enemies with Temporal Chains on Hit
Non-Aura Curses you inflict are not removed from Dying Enemies
Enemies near corpses affected by your Curses are Blinded
Enemies Killed near corpses affected by your Curses explode, dealing
3% of their Life as Physical Damage
--------
Cool the head and cool the blade.
--------
Corrupted
");

        Assert.Equal(ItemClass.Gloves, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Silk Gloves", actual.TradeItem?.Type);
        Assert.Equal("Asenath's Gentle Touch", actual.Name);
        Assert.NotNull(actual.Properties.Sockets);
        Assert.Single(actual.Properties.Sockets);

        var stat = fixture.AssertHasStat(actual, StatCategory.Implicit, "Curse Enemies with Enfeeble on Hit");
        Assert.NotNull(stat);
        Assert.Empty(stat.Values);
    }

    [Fact]
    public void DoryanisFist()
    {
        var actual = parser.ParseItem(@"Item Class: Gloves
Rarity: Unique
Doryani's Fist
Vaal Gauntlets
--------
Armour: 236
Energy Shield: 80 (augmented)
--------
Requirements:
Level: 63
Str: 100 (unmet)
--------
Sockets: W-W-W 
--------
Item Level: 80
--------
{ Unique Modifier — Damage, Elemental, Lightning, Attack }
Adds 190(150-225) to 550(525-600) Lightning Damage to Unarmed Melee Hits
(Unarmed Attacks are any Attacks performed with a hand that is not holding any Item)
{ Unique Modifier — Damage, Elemental, Lightning, Caster }
Adds 99(90-135) to 344(315-360) Lightning Damage to Spells while Unarmed
(You are Unarmed as long as you have no Equipped Weapons)
{ Unique Modifier — Defences, Energy Shield }
+204(200-250) Energy Shield gained on Killing a Shocked Enemy
{ Unique Modifier — Defences, Energy Shield }
+80(80-100) to maximum Energy Shield
{ Unique Modifier — Elemental, Lightning, Ailment }
30% chance to Shock
(Shock increases Damage taken by up to 50%, depending on the amount of Lightning Damage in the hit, for 2 seconds)
{ Unique Modifier }
Grants Level 20 Doryani's Touch Skill
{ Unique Modifier — Elemental, Lightning, Ailment }
30% increased Effect of Lightning Ailments
(Lightning Ailments are Shocked and Sapped)
--------
Doryani's ingenuity raised the Vaal Empire
to unprecedented heights. His curiosity
reduced it to ruins and bones.
");

        Assert.Equal(ItemClass.Gloves, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Vaal Gauntlets", actual.TradeItem?.Type);
        Assert.Equal("Doryani's Fist", actual.Name);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "Adds # to # Lightning Damage to Unarmed Melee Hits", 190, 550);
    }

}
