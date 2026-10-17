using System.Text.Json;
using Moq;
using Sidekick.Common.Settings;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Filters.AutoSelect;

/// <summary>
///     Makes sure a rule using one of the new item class options matches items of that class.
/// </summary>
public class AutoSelectItemClassConditionTests
{
    private class TestFilter : TradeFilter;

    private readonly ISettingsService settingsService = new Mock<ISettingsService>().Object;

    private static async Task<Item> CreateItem(GameType game, ItemClass itemClass)
    {
        var definitions = await GameData.GetItemClasses(game);
        return new Item(game, new OriginalText(string.Empty))
        {
            ItemClass = definitions.First(x => x.Type == itemClass),
        };
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

    [Theory]
    [InlineData(GameType.Poe1, ItemClass.Tincture)]
    [InlineData(GameType.Poe1, ItemClass.Trinket)]
    [InlineData(GameType.Poe1, ItemClass.HeistContract)]
    [InlineData(GameType.Poe2, ItemClass.Spear)]
    [InlineData(GameType.Poe2, ItemClass.Focus)]
    [InlineData(GameType.Poe2, ItemClass.LifeFlask)]
    public async Task RuleMatchesItemOfSelectedClass(GameType game, ItemClass itemClass)
    {
        var item = await CreateItem(game, itemClass);

        var result = await CreatePreferences(ItemClass.Amulet, itemClass).GetResult(item, new TestFilter(), settingsService);

        Assert.True(result.Checked);
    }

    [Theory]
    [InlineData(GameType.Poe1, ItemClass.Tincture, ItemClass.Flask)]
    [InlineData(GameType.Poe2, ItemClass.Spear, ItemClass.Talisman)]
    public async Task RuleDoesNotMatchItemOfOtherClass(GameType game, ItemClass itemClass, ItemClass selected)
    {
        var item = await CreateItem(game, itemClass);

        var result = await CreatePreferences(selected).GetResult(item, new TestFilter(), settingsService);

        Assert.False(result.Checked);
    }
}
