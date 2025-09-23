using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class AbyssParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void BulbonicTrail()
    {
        var actual = parser.ParseItem(@"Item Class: Boots
Rarity: Unique
Bubonic Trail
Murder Boots
--------
Evasion Rating: 185
Energy Shield: 17
--------
Requirements:
Level: 69
Dex: 82
Int: 42
--------
Sockets: G-G A
--------
Item Level: 84
--------
Has 1 Abyssal Socket
Triggers Level 20 Death Walk when Equipped
6% increased maximum Life
30% increased Movement Speed
10% increased Damage for each type of Abyss Jewel affecting you
--------
Even the dead serve the Lightless.
");

        Assert.Equal(ItemClass.Boots, actual.Properties.ItemClass);
        Assert.Equal(Category.Armour, actual.Header.Category);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Bubonic Trail", actual.Header.Name);
        Assert.Equal("Murder Boots", actual.Header.Type);

        actual.AssertHasModifier(ModifierCategory.Explicit, "Has # Abyssal Sockets", 1);
    }

    [Fact]
    public void AbyssJewel()
    {
        var actual = parser.ParseItem(@"Item Class: Abyss Jewels
Rarity: Rare
Whispering Leer
Hypnotic Eye Jewel
--------
Abyss
--------
Requirements:
Level: 52
--------
Item Level: 69
--------
Adds 12 to 18 Fire Damage to Spells
Adds 16 to 23 Cold Damage to Spells
2 to 20 Added Spell Lightning Damage while wielding a Two Handed Weapon
9 to 15 Added Spell Physical Damage while wielding a Two Handed Weapon
--------
Place into an Abyssal Socket on an Item or into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.
--------
Note: ~price 1 alch
");

        Assert.Equal(ItemClass.AbyssJewel, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal(Category.Jewel, actual.Header.Category);
        Assert.Equal("Hypnotic Eye Jewel", actual.Header.Type);
    }
}
