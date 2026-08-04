using Sidekick.Common.Enums;
namespace Sidekick.Data.Stats;

public static class StatCategoryExtensions
{
    public static StatCategory GetStatCategory(this string id)
    {
        var value = id.Split('.').First();
        return value.GetEnumFromValue<StatCategory>();
    }

    public static bool HasExplicitStat(this StatCategory category)
    {
        return category switch
        {
            StatCategory.Crafted => true,
            StatCategory.Desecrated => true,
            StatCategory.Fractured => true,
            _ => false,
        };
    }
}

public enum StatCategory
{
    Undefined = 0,

    [EnumValue("pseudo")]
    Pseudo = 1,

    [EnumValue("explicit")]
    Explicit = 2,

    [EnumValue("implicit")]
    Implicit = 3,

    [EnumValue("imbued")]
    Imbued = 4,

    [EnumValue("fractured")]
    Fractured = 5,

    [EnumValue("enchant")]
    Enchant = 6,

    [EnumValue("scourge")]
    Scourge = 7,

    [EnumValue("crafted")]
    Crafted = 8,

    [EnumValue("mercenary")]
    Mercenary = 9,

    [EnumValue("veiled")]
    Veiled = 10,

    [EnumValue("delve")]
    Delve = 11,

    [EnumValue("ultimatum")]
    Uultimatum = 12,

    [EnumValue("sanctum")]
    Sanctum = 13,

    [EnumValue("crucible")]
    Crucible = 14,

    [EnumValue("rune")]
    Rune = 15,

    [EnumValue("desecrated")]
    Desecrated = 16,

    [EnumValue("skill")]
    Skill = 17,

    // Meta modifiers
    Corrupted = 101,
    Unidentified = 102,
    WhiteText = 103,
    GrayText = 104,
    Mutated = 105,

    // Logbook modifiers
    DruidsOfTheBrokenCircle = 201,
    BlackScytheMercenaries = 202,
    OrderOfTheChalice = 203,
    KnightsOfTheSun = 204,
}
