using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe2.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class HelmetParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void Ironride()
    {
        var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Unique
Ironride
Visored Helm
--------
Quality: +20% (augmented)
Armour: 105 (augmented)
Evasion Rating: 89 (augmented)
--------
Requires: Level 16, 14 (augmented) Str, 14 (augmented) Dex
--------
Sockets: S 
--------
Item Level: 81
--------
{ Corruption Enhancement — Mana }
30(20-30)% increased Mana Regeneration Rate
--------
{ Unique Modifier — Armour, Evasion }
62(60-80)% increased Armour and Evasion
{ Unique Modifier — Elemental, Lightning, Resistance }
+10(10-15)% to Lightning Resistance
{ Unique Modifier — Mana }
+40(30-50) to maximum Mana
{ Unique Modifier }
You have no Accuracy Penalty at Distance — Unscalable Value
{ Unique Modifier — Life }
+44(30-50) to maximum Life
--------
Let the rider's aim be true.
--------
Corrupted");

        Assert.Equal(ItemClass.Helmet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Ironride", actual.TradeItem?.Name);
        Assert.Equal("Visored Helm", actual.TradeItem?.Type);

        Assert.Equal(81, actual.Properties.ItemLevel);
        Assert.Equal(105, actual.Properties.Armour);
        Assert.Equal(89, actual.Properties.EvasionRating);

        Assert.Equal(16, actual.Properties.RequiresLevel);
        Assert.Equal(14, actual.Properties.RequiresStrength);
        Assert.Equal(14, actual.Properties.RequiresDexterity);

        fixture.AssertHasStat(actual, StatCategory.Enchant, "#% increased Mana Regeneration Rate", 30);
    }
}
