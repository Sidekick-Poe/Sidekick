using System.Text.Json;
using Sidekick.Game;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Filters;

[Collection(Collections.Poe1EnglishFixture)]
public class ItemClassConditionOptions(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    private List<AutoSelectItemClassOption> GetOptions() => AutoSelectItemClassOptions.Get(GameType.Poe1, fixture.ItemClassProvider.Definitions);

    [Theory]
    [InlineData(ItemClass.Amulet)]
    [InlineData(ItemClass.AbyssJewel)]
    [InlineData(ItemClass.Claw)]
    [InlineData(ItemClass.Flask)]
    [InlineData(ItemClass.Warstaff)]
    [InlineData(ItemClass.Trinket)]
    [InlineData(ItemClass.Tincture)]
    [InlineData(ItemClass.Charms)]
    [InlineData(ItemClass.Idol)]
    [InlineData(ItemClass.SanctumRelic)]
    [InlineData(ItemClass.Map)]
    [InlineData(ItemClass.Chart)]
    [InlineData(ItemClass.ExpeditionLogbook)]
    [InlineData(ItemClass.HeistContract)]
    [InlineData(ItemClass.HeistBlueprint)]
    [InlineData(ItemClass.HeistEquipmentWeapon)]
    [InlineData(ItemClass.HeistEquipmentTool)]
    [InlineData(ItemClass.HeistEquipmentUtility)]
    [InlineData(ItemClass.HeistEquipmentReward)]
    public void ContainsItemClass(ItemClass expected)
    {
        Assert.Contains(GetOptions(), x => x.Type == expected);
    }

    [Theory]
    [InlineData(ItemClass.Unknown)]
    [InlineData(ItemClass.Crossbow)]
    [InlineData(ItemClass.Focus)]
    [InlineData(ItemClass.Buckler)]
    [InlineData(ItemClass.Spear)]
    [InlineData(ItemClass.Flail)]
    [InlineData(ItemClass.Talisman)]
    [InlineData(ItemClass.LifeFlask)]
    [InlineData(ItemClass.ManaFlask)]
    public void DoesNotContainItemClassFromOtherGame(ItemClass unexpected)
    {
        Assert.DoesNotContain(GetOptions(), x => x.Type == unexpected);
    }

    [Fact]
    public void OptionsAreUnique()
    {
        var options = GetOptions();
        Assert.Equal(options.Count, options.Select(x => x.Type).Distinct().Count());
    }

    [Theory]
    [InlineData(ItemClass.Map, "Maps")]
    [InlineData(ItemClass.SanctumRelic, "Relics")]
    [InlineData(ItemClass.Dagger, "Daggers")]
    [InlineData(ItemClass.OneHandSword, "One Hand Swords")]
    public void SharedItemClassLabel(ItemClass type, string expectedLabel)
    {
        Assert.Equal(expectedLabel, GetOptions().Single(x => x.Type == type).Label);
    }

    [Fact]
    public async Task ConditionMatchesTincture()
    {
        var item = parser.ParseItem(@"Item Class: Tinctures
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
        var filter = (await fixture.GetPropertyFilters(item)).First();

        var matching = await CreatePreferences(ItemClass.Tincture).GetResult(item, filter, fixture.SettingsService);
        var notMatching = await CreatePreferences(ItemClass.Flask).GetResult(item, filter, fixture.SettingsService);

        Assert.True(matching.Checked);
        Assert.False(notMatching.Checked);
    }

    private static AutoSelectPreferences CreatePreferences(params ItemClass[] itemClasses) => new()
    {
        Mode = AutoSelectMode.Default,
        Rules =
        [
            new AutoSelectRule
            {
                Checked = true,
                Conditions =
                [
                    new AutoSelectCondition
                    {
                        Type = AutoSelectConditionType.ItemClass,
                        Comparison = AutoSelectComparisonType.IsContainedIn,
                        Value = JsonSerializer.Serialize(itemClasses.ToList(), AutoSelectPreferences.JsonSerializerOptions),
                    },
                ],
            },
        ],
    };
}
