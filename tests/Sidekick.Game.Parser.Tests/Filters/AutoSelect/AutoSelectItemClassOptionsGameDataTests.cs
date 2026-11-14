using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Filters.AutoSelect;

/// <summary>
///     Builds the item class options from the English game data files, to make sure each game offers its own item classes.
/// </summary>
public class AutoSelectItemClassOptionsGameDataTests
{
    private static async Task<List<AutoSelectItemClassOption>> GetOptions(GameType game)
    {
        return AutoSelectItemClassOptions.Get(game, await GameData.GetItemClasses(game));
    }

    [Theory]
    [InlineData(GameType.Poe1, ItemClass.Amulet)]
    [InlineData(GameType.Poe1, ItemClass.AbyssJewel)]
    [InlineData(GameType.Poe1, ItemClass.Claw)]
    [InlineData(GameType.Poe1, ItemClass.Flask)]
    [InlineData(GameType.Poe1, ItemClass.Warstaff)]
    [InlineData(GameType.Poe1, ItemClass.Trinket)]
    [InlineData(GameType.Poe1, ItemClass.Tincture)]
    [InlineData(GameType.Poe1, ItemClass.Charms)]
    [InlineData(GameType.Poe1, ItemClass.Idol)]
    [InlineData(GameType.Poe1, ItemClass.SanctumRelic)]
    [InlineData(GameType.Poe1, ItemClass.Map)]
    [InlineData(GameType.Poe1, ItemClass.Chart)]
    [InlineData(GameType.Poe1, ItemClass.ExpeditionLogbook)]
    [InlineData(GameType.Poe1, ItemClass.HeistContract)]
    [InlineData(GameType.Poe1, ItemClass.HeistBlueprint)]
    [InlineData(GameType.Poe1, ItemClass.HeistEquipmentWeapon)]
    [InlineData(GameType.Poe1, ItemClass.HeistEquipmentTool)]
    [InlineData(GameType.Poe1, ItemClass.HeistEquipmentUtility)]
    [InlineData(GameType.Poe1, ItemClass.HeistEquipmentReward)]
    [InlineData(GameType.Poe2, ItemClass.Amulet)]
    [InlineData(GameType.Poe2, ItemClass.Crossbow)]
    [InlineData(GameType.Poe2, ItemClass.Warstaff)]
    [InlineData(GameType.Poe2, ItemClass.Focus)]
    [InlineData(GameType.Poe2, ItemClass.Buckler)]
    [InlineData(GameType.Poe2, ItemClass.Spear)]
    [InlineData(GameType.Poe2, ItemClass.Flail)]
    [InlineData(GameType.Poe2, ItemClass.Talisman)]
    [InlineData(GameType.Poe2, ItemClass.LifeFlask)]
    [InlineData(GameType.Poe2, ItemClass.ManaFlask)]
    [InlineData(GameType.Poe2, ItemClass.Charms)]
    [InlineData(GameType.Poe2, ItemClass.Tablet)]
    [InlineData(GameType.Poe2, ItemClass.Map)]
    [InlineData(GameType.Poe2, ItemClass.SanctumRelic)]
    public async Task ContainsItemClass(GameType game, ItemClass expected)
    {
        Assert.Contains(await GetOptions(game), x => x.Type == expected);
    }

    [Theory]
    [InlineData(GameType.Poe1, ItemClass.Unknown)]
    [InlineData(GameType.Poe1, ItemClass.Crossbow)]
    [InlineData(GameType.Poe1, ItemClass.Focus)]
    [InlineData(GameType.Poe1, ItemClass.Buckler)]
    [InlineData(GameType.Poe1, ItemClass.Spear)]
    [InlineData(GameType.Poe1, ItemClass.Flail)]
    [InlineData(GameType.Poe1, ItemClass.Talisman)]
    [InlineData(GameType.Poe1, ItemClass.LifeFlask)]
    [InlineData(GameType.Poe1, ItemClass.ManaFlask)]
    [InlineData(GameType.Poe2, ItemClass.Unknown)]
    [InlineData(GameType.Poe2, ItemClass.AbyssJewel)]
    [InlineData(GameType.Poe2, ItemClass.Flask)]
    [InlineData(GameType.Poe2, ItemClass.Trinket)]
    [InlineData(GameType.Poe2, ItemClass.Tincture)]
    [InlineData(GameType.Poe2, ItemClass.Idol)]
    [InlineData(GameType.Poe2, ItemClass.HeistContract)]
    public async Task DoesNotContainItemClass(GameType game, ItemClass unexpected)
    {
        Assert.DoesNotContain(await GetOptions(game), x => x.Type == unexpected);
    }

    [Theory]
    [InlineData(GameType.Poe1)]
    [InlineData(GameType.Poe2)]
    public async Task OptionsAreUnique(GameType game)
    {
        var options = await GetOptions(game);
        Assert.Equal(options.Count, options.Select(x => x.Type).Distinct().Count());
    }

    [Theory]
    [InlineData(GameType.Poe1, ItemClass.Map, "Maps")]
    [InlineData(GameType.Poe1, ItemClass.SanctumRelic, "Relics")]
    [InlineData(GameType.Poe1, ItemClass.Dagger, "Daggers")]
    [InlineData(GameType.Poe1, ItemClass.OneHandSword, "One Hand Swords")]
    [InlineData(GameType.Poe2, ItemClass.SanctumRelic, "Relics")]
    public async Task SharedItemClassLabel(GameType game, ItemClass type, string expectedLabel)
    {
        Assert.Equal(expectedLabel, (await GetOptions(game)).Single(x => x.Type == type).Label);
    }
}
