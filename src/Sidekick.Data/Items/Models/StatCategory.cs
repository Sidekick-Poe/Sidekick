using Sidekick.Common.Enums;
namespace Sidekick.Data.Items.Models;

public static class StatCategoryExtensions
{
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

    public static List<StatCategory> ExplicitCategories { get; } =
    [
        StatCategory.Explicit,
        StatCategory.Delve,
        StatCategory.Sanctum,
    ];
}

public enum StatCategory
{

    Undefined = 0,

    [EnumValue("crafted")]
    Crafted = 1,

    [EnumValue("delve")]
    Delve = 2,

    [EnumValue("enchant")]
    Enchant = 3,

    [EnumValue("explicit")]
    Explicit = 4,

    [EnumValue("fractured")]
    Fractured = 5,

    [EnumValue("implicit")]
    Implicit = 6,

    [EnumValue("monster")]
    Monster = 7,

    [EnumValue("pseudo")]
    Pseudo = 8,

    [EnumValue("scourge")]
    Scourge = 9,

    [EnumValue("veiled")]
    Veiled = 10,

    [EnumValue("crucible")]
    Crucible = 11,

    [EnumValue("rune")]
    Rune = 12,

    [EnumValue("sanctum")]
    Sanctum = 13,

    [EnumValue("desecrated")]
    Desecrated = 14,

    [EnumValue("skill")]
    Skill = 15,

    [EnumValue("mutated")]
    Mutated = 16,

    // Meta modifiers
    Corrupted = 101,
    Unidentified = 102,
    WhiteText = 103,
    GrayText = 104,

    // Logbook modifiers
    DruidsOfTheBrokenCircle = 201,
    BlackScytheMercenaries = 202,
    OrderOfTheChalice = 203,
    KnightsOfTheSun = 204,
}
