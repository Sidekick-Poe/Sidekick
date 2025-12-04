using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class HelmetParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseBlightGuardian()
    {
        var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Rare
Blight Guardian
Hunter Hood
--------
Evasion Rating: 231 (augmented)
--------
Requirements:
Level: 64
Dex: 87
--------
Sockets: G
--------
Item Level: 80
--------
Adds 28 to 51 Fire Damage to Spells
+28 to Evasion Rating
+47 to maximum Life
11% increased Rarity of Items found
+29% to Cold Resistance
You have Shocking Conflux for 3 seconds every 8 seconds
--------
Hunter Item
");

        Assert.Equal(ItemClass.Helmet, actual.Properties.ItemClass);
        Assert.Equal(Category.Armour, actual.ApiInformation.Category);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Hunter Hood", actual.ApiInformation.Type);

        actual.AssertHasModifier(ModifierCategory.Explicit, "You have Shocking Conflux for 3 seconds every 8 seconds");
    }

    [Fact]
    public void ParseStarkonjaHead()
    {
        var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Unique
Starkonja's Head
Silken Hood
--------
Evasion Rating: 793 (augmented)
--------
Requirements:
Level: 60
Dex: 138
--------
Sockets: G
--------
Item Level: 63
--------
+53 to Dexterity
50% reduced Damage when on Low Life
10% increased Attack Speed
25% increased Global Critical Strike Chance
124% increased Evasion Rating
+80 to maximum Life
150% increased Global Evasion Rating when on Low Life
--------
There was no hero made out of Starkonja's death,
but merely a long sleep made eternal.
--------
Note: ~price 1 chaos
");

        Assert.Equal(ItemClass.Helmet, actual.Properties.ItemClass);
        Assert.Equal(Category.Armour, actual.ApiInformation.Category);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Starkonja's Head", actual.ApiInformation.Name);
        Assert.Equal("Silken Hood", actual.ApiInformation.Type);

        Assert.False(actual.Properties.Unidentified);
        Assert.Equal(63, actual.Properties.ItemLevel);
        Assert.Equal(793, actual.Properties.EvasionRating);

        actual.AssertHasModifier(ModifierCategory.Explicit, "+# to Dexterity", 53);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Damage when on Low Life", -50);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Attack Speed", 10);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Global Critical Strike Chance", 25);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Evasion Rating", 124);
        actual.AssertHasModifier(ModifierCategory.Explicit, "+# to maximum Life", 80);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Global Evasion Rating when on Low Life", 150);
    }
}
