using Sidekick.Game.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class UniqueJewelTests(NinjaTestFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void BlueDream()
    {
        var item = parser.ParseItem(@"Item Class: Jewels
Rarity: Unique
Foulborn The Blue Dream
Cobalt Jewel
--------
Limited to: 1
Radius: Large
--------
Item Level: 83
--------
+3% to Critical Strike Multiplier per Power Charge (mutated)
Passives granting Lightning Resistance or all Elemental Resistances in Radius
also grant an equal chance to gain a Power Charge on Kill
--------
We crash against Chayula's body,
and fall like rain into the place we cannot go.
--------
Place into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.
");

        fixture.AssertStash(item, "foulborn-the-blue-dream-crit-multi-cobalt-jewel");
    }
}
