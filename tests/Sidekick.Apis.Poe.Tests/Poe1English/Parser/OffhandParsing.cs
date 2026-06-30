using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class OffhandParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void AdvancedStatParsing()
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
{ Prefix Modifier ""Seraphim's"" (Tier: 1) — Defences, Energy Shield }
42(39-42)% increased Energy Shield
17(16-17)% increased Stun and Block Recovery
{ Prefix Modifier ""Enduring"" (Tier: 2) }
75(70-75)% increased Chance to Block
{ Prefix Modifier ""Unfaltering"" (Tier: 1) — Defences, Energy Shield }
109(101-110)% increased Energy Shield
{ Suffix Modifier ""of Steel Skin"" (Tier: 3) }
22(20-22)% increased Stun and Block Recovery
");

        Assert.Equal(ItemClass.Shield, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Titanium Spirit Shield", actual.Definition.TradeItem?.Type);

        Assert.Equal(86, actual.Properties.ItemLevel);
        Assert.False(actual.Properties.Unidentified);
        Assert.False(actual.Properties.Corrupted);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Energy Shield (Local)", 151);
    }

}
