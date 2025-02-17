using Sidekick.Common.Game.Items;
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

        Assert.Equal("flask", actual.Header.ItemCategory);
        Assert.Equal(Category.Flask, actual.Header.Category);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Equal("Sanctified Mana Flask", actual.Header.ApiType);

        actual.AssertHasModifier(ModifierCategory.Explicit, "Grants Immunity to Bleeding for 4 seconds if used while Bleeding\nGrants Immunity to Corrupted Blood for 4 seconds if used while affected by Corrupted Blood", 4);
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

        Assert.Equal("flask", actual.Header.ItemCategory);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal(Category.Flask, actual.Header.Category);
        Assert.Equal("Hallowed Life Flask", actual.Header.ApiType);
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

        Assert.Equal("flask", actual.Header.ItemCategory);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal(Category.Flask, actual.Header.Category);
        Assert.Equal("Sacred Hybrid Flask", actual.Header.ApiType);
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

        Assert.Equal("tincture", actual.Header.ItemCategory);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal(Category.Tincture, actual.Header.Category);
        Assert.Equal("Poisonberry Tincture", actual.Header.ApiType);
    }
}
