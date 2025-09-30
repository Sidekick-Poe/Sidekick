using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class BlightParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseBlightedMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blighted Atoll Map
--------
Map Tier: 14
Atlas Region: Tirn's End
--------
Item Level: 81
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 20% value (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(Category.Map, actual.ApiInformation.Category);
        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Blighted Atoll Map", actual.Type);
        Assert.Equal("Blighted Atoll Map", actual.ApiInformation.Text);
        Assert.Equal("Atoll Map", actual.ApiInformation.Type);
        Assert.Equal("blighted", actual.ApiInformation.Discriminator);
        Assert.Equal(14, actual.Properties.MapTier);
        Assert.True(actual.Properties.Blighted);
    }

    [Fact]
    public void ParseSuperiorBlightedMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Superior Blighted Shore Map
--------
Map Tier: 6
Item Quantity: +5% (augmented)
Quality: +5% (augmented)
--------
Item Level: 74
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 25% value (implicit)
Can be Anointed up to 3 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(Category.Map, actual.ApiInformation.Category);
        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Blighted Shore Map", actual.ApiInformation.Text);
        Assert.Equal("Shore Map", actual.ApiInformation.Type);
        Assert.Equal("blighted", actual.ApiInformation.Discriminator);
        Assert.Null(actual.Name);
        Assert.Equal(6, actual.Properties.MapTier);
        Assert.True(actual.Properties.Blighted);
    }

    [Fact]
    public void ClearOil()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Clear Oil
--------
Stack Size: 5/10
--------
Can be combined with other Oils at Cassia to Enchant Rings or Amulets, or to modify Blighted Maps.
Shift click to unstack.
--------
Note: ~price 1 blessed
");

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal(Category.Currency, actual.ApiInformation.Category);
        Assert.Equal("Clear Oil", actual.ApiInformation.Type);
    }

    [Fact]
    public void BlightedSpiderForestMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Nightmare Spires
Blighted Spider Forest Map
--------
Map Tier: 2
Atlas Region: Lex Proxima
Item Quantity: +55% (augmented)
Item Rarity: +32% (augmented)
Monster Pack Size: +21% (augmented)
--------
Item Level: 69
--------
Area is infested with Fungal Growths
Map's Item Quantity Modifiers also affect Blight Chest count at 20% value (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Monsters deal 54% extra Physical Damage as Lightning
Unique Boss has 25% increased Life
Unique Boss has 45% increased Area of Effect
Slaying Enemies close together has a 13% chance to attract monsters from Beyond
Players have 20% less Recovery Rate of Life and Energy Shield
--------
Travel to this Map by using it in a personal Map Device.Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal(Category.Map, actual.ApiInformation.Category);
        Assert.Equal("Blighted Spider Forest Map", actual.ApiInformation.Text);
        Assert.Equal("Spider Forest Map", actual.ApiInformation.Type);
        Assert.Equal("blighted", actual.ApiInformation.Discriminator);
        Assert.Equal("Nightmare Spires", actual.Name);
        Assert.True(actual.Properties.Blighted);
    }
}
