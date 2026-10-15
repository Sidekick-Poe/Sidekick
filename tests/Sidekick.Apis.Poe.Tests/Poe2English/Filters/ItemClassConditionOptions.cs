using System.Text.Json;
using Sidekick.Game;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Filters;

[Collection(Collections.Poe2EnglishFixture)]
public class ItemClassConditionOptions(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    private List<AutoSelectItemClassOption> GetOptions() => AutoSelectItemClassOptions.Get(GameType.Poe2, fixture.ItemClassProvider.Definitions);

    [Theory]
    [InlineData(ItemClass.Amulet)]
    [InlineData(ItemClass.Crossbow)]
    [InlineData(ItemClass.Warstaff)]
    [InlineData(ItemClass.Focus)]
    [InlineData(ItemClass.Buckler)]
    [InlineData(ItemClass.Spear)]
    [InlineData(ItemClass.Flail)]
    [InlineData(ItemClass.Talisman)]
    [InlineData(ItemClass.LifeFlask)]
    [InlineData(ItemClass.ManaFlask)]
    [InlineData(ItemClass.Charms)]
    [InlineData(ItemClass.Tablet)]
    [InlineData(ItemClass.Map)]
    [InlineData(ItemClass.SanctumRelic)]
    public void ContainsItemClass(ItemClass expected)
    {
        Assert.Contains(GetOptions(), x => x.Type == expected);
    }

    [Theory]
    [InlineData(ItemClass.Unknown)]
    [InlineData(ItemClass.AbyssJewel)]
    [InlineData(ItemClass.Flask)]
    [InlineData(ItemClass.Trinket)]
    [InlineData(ItemClass.Tincture)]
    [InlineData(ItemClass.Idol)]
    [InlineData(ItemClass.HeistContract)]
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

    [Fact]
    public async Task ConditionMatchesSpear()
    {
        var item = parser.ParseItem(@"Item Class: Spears
Rarity: Magic
Precise Ironhead Spear
--------
Physical Damage: 7-13
Critical Hit Chance: 5.00%
Attacks per Second: 1.60
--------
Requires: Level 5, 11 Dex
--------
Item Level: 5
--------
Grants Skill: Spear Throw
--------
+32 to Accuracy Rating

");
        var filter = (await fixture.GetPropertyFilters(item)).First();

        var matching = await CreatePreferences(ItemClass.Spear, ItemClass.Talisman).GetResult(item, filter, fixture.SettingsService);
        var notMatching = await CreatePreferences(ItemClass.Talisman).GetResult(item, filter, fixture.SettingsService);

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
