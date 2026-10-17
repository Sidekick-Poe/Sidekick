using Sidekick.Game;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Filters.AutoSelect;

public class AutoSelectItemClassOptionsTests
{
    private static ItemClassDefinition Definition(string id, ItemClass type, string? name = null) => new()
    {
        Id = id,
        Type = type,
        Name = name ?? id,
    };

    [Fact]
    public void ExcludesUnknownItemClasses()
    {
        var options = AutoSelectItemClassOptions.Get(GameType.Poe1,
        [
            Definition("Amulet", ItemClass.Amulet, "Amulets"),
            Definition("ArchnemesisMod", ItemClass.Unknown, "Archnemesis Mods"),
        ]);

        var option = Assert.Single(options);
        Assert.Equal(ItemClass.Amulet, option.Type);
        Assert.Equal("Amulets", option.Label);
    }

    [Fact]
    public void ReturnsOneOptionPerItemClass()
    {
        var options = AutoSelectItemClassOptions.Get(GameType.Poe1,
        [
            Definition("Dagger", ItemClass.Dagger, "Daggers"),
            Definition("Rune Dagger", ItemClass.Dagger, "Rune Daggers"),
        ]);

        var option = Assert.Single(options);
        Assert.Equal(ItemClass.Dagger, option.Type);
    }

    [Fact]
    public void SharedItemClassUsesThePrimaryGameIdForTheLabel()
    {
        // The primary game id of ItemClass.Map is "Map", even if another definition comes first in the data.
        var options = AutoSelectItemClassOptions.Get(GameType.Poe1,
        [
            Definition("InstanceLocalItem", ItemClass.Map, "Instance Local Items"),
            Definition("Map", ItemClass.Map, "Maps"),
            Definition("MapKey", ItemClass.Map, "Maps"),
        ]);

        Assert.Equal("Maps", Assert.Single(options).Label);
    }

    [Fact]
    public void SharedItemClassWithoutPrimaryGameIdFallsBackToTheEnumName()
    {
        var options = AutoSelectItemClassOptions.Get(GameType.Poe1,
        [
            Definition("HybridFlask", ItemClass.Flask, "Hybrid Flasks"),
            Definition("LifeFlask", ItemClass.Flask, "Life Flasks"),
        ]);

        Assert.Equal(nameof(ItemClass.Flask), Assert.Single(options).Label);
    }

    [Fact]
    public void OptionsAreSortedByLabel()
    {
        var options = AutoSelectItemClassOptions.Get(GameType.Poe2,
        [
            Definition("Wand", ItemClass.Wand, "Wands"),
            Definition("Amulet", ItemClass.Amulet, "Amulets"),
            Definition("Focus", ItemClass.Focus, "Foci"),
        ]);

        Assert.Equal(["Amulets", "Foci", "Wands"], options.Select(x => x.Label));
    }

    [Fact]
    public void SelectedValuesMissingFromTheGameDataAreKept()
    {
        // A rule saved with an item class that the current game data does not have must stay visible, so it can be unchecked.
        var options = AutoSelectItemClassOptions.Get(GameType.Poe2,
                                                     [Definition("Amulet", ItemClass.Amulet, "Amulets")],
                                                     [nameof(ItemClass.Amulet), nameof(ItemClass.AbyssJewel)]);

        Assert.Equal(2, options.Count);
        Assert.Contains(options, x => x.Type == ItemClass.AbyssJewel && x.Label == nameof(ItemClass.AbyssJewel));
    }

    [Theory]
    [InlineData("NotAnItemClass")]
    [InlineData("12345")]
    [InlineData("Unknown")]
    [InlineData("null")]
    [InlineData("")]
    public void InvalidSelectedValuesAreIgnored(string value)
    {
        var options = AutoSelectItemClassOptions.Get(GameType.Poe1,
                                                     [Definition("Amulet", ItemClass.Amulet, "Amulets")],
                                                     [value]);

        Assert.Equal(ItemClass.Amulet, Assert.Single(options).Type);
    }
}
