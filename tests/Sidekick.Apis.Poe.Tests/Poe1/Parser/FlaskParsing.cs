using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class FlaskParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseSanctifiedManaFlask()
    {
        var actual = parser.ParseItem(@"Item Class: Mana Flasks
Rarity: Magic
Sanctified Mana Flask of Staunching
--------
Quality: +7% (augmented)
Recovers 1177 (augmented) Mana over 6.50 Seconds
Consumes 7 of 35 Charges on use
Currently has 0 Charges
--------
Requirements:
Level: 50
--------
Item Level: 72
--------
Grants Immunity to Bleeding for 4 seconds if used while Bleeding
Grants Immunity to Corrupted Blood for 4 seconds if used while affected by Corrupted Blood
--------
Right click to drink. Can only hold charges while in belt. Refills as you kill monsters.
");

        Assert.Equal(ItemClass.Flask, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Sanctified Mana Flask", actual.ApiInformation.Type);

        actual.AssertHasModifier(ModifierCategory.Explicit, "Grants Immunity to Bleeding for 4 seconds if used while Bleeding\nGrants Immunity to Corrupted Blood for 4 seconds if used while affected by Corrupted Blood", 4, 4);
    }

    [Fact]
    public void HallowedLifeFlask()
    {
        var actual = parser.ParseItem(@"Item Class: Life Flasks
Rarity: Normal
Hallowed Life Flask
--------
Recovers 1990 Life over 8 Seconds
Consumes 10 of 30 Charges on use
Currently has 0 Charges
--------
Requirements:
Level: 42
--------
Item Level: 42
--------
Right click to drink. Can only hold charges while in belt. Refills as you kill monsters.
");

        Assert.Equal(ItemClass.Flask, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Hallowed Life Flask", actual.ApiInformation.Type);
    }

    [Fact]
    public void SacredHybridFlask()
    {
        var actual = parser.ParseItem(@"Item Class: Hybrid Flasks
Rarity: Normal
Superior Sacred Hybrid Flask
--------
Quality: +13% (augmented)
Recovers 1627 (augmented) Life over 5 Seconds
Recovers 452 (augmented) Mana over 5 Seconds
Consumes 20 of 40 Charges on use
Currently has 0 Charges
--------
Requirements:
Level: 50
--------
Item Level: 76
--------
Right click to drink. Can only hold charges while in belt. Refills as you kill monsters.
");

        Assert.Equal(ItemClass.Flask, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Sacred Hybrid Flask", actual.ApiInformation.Type);
        Assert.Equal(13, actual.Properties.Quality);
    }

    [Fact]
    public void Tincture()
    {
        var actual = parser.ParseItem(@"Item Class: Tinctures
Rarity: Normal
Poisonberry Tincture
--------
Inflicts Mana Burn every 0.70 Seconds
6 Second Cooldown when Deactivated
--------
Requirements:
Level: 45
--------
Item Level: 82
--------
20% chance to Poison with Melee Weapons (implicit)
70% increased Damage with Poison from Melee Weapons (implicit)
--------
Right click to activate. Only one Tincture in your belt can be active at a time. Mana Burn causes you to lose 1% of your maximum Mana per stack per second. Can be deactivated manually, or will automatically deactivate when you reach 0 Mana.
");

        Assert.Equal(ItemClass.Tincture, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Poisonberry Tincture", actual.ApiInformation.Type);
    }

    [Fact]
    public void FlagellantMod()
    {
        var actual = parser.ParseItem(@"Item Class: Utility Flasks
Rarity: Magic
Flagellant's Bismuth Flask of the Rabbit
--------
Lasts 8.50 Seconds
Consumes 15 of 40 Charges on use
Currently has 0 Charges
+35% to all Elemental Resistances
--------
Requirements:
Level: 64
--------
Item Level: 84
--------
Gain 3 Charges when you are Hit by an Enemy
40% reduced Effect of Chill on you during Effect
41% reduced Freeze Duration on you during Effect
--------
Right click to drink. Can only hold charges while in belt. Refills as you kill monsters.");

        Assert.Equal(ItemClass.Flask, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Bismuth Flask", actual.ApiInformation.Type);

        actual.AssertHasModifier(ModifierCategory.Explicit, "Gain # Charge when you are Hit by an Enemy", 3);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% reduced Effect of Chill on you during Effect", 40);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Freeze Duration on you during Effect", 41);
    }
}
