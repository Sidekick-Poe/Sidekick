using Sidekick.Common.Enums;

namespace Sidekick.Common.Game.Items;

public static class ModifierCategoryExtensions
{
    public static readonly List<ModifierCategory> AllExplicitCategories =
    [
        ModifierCategory.Explicit,
        ModifierCategory.Fractured,
        ModifierCategory.Desecrated,
        ModifierCategory.Crafted,
        ModifierCategory.Delve,
        ModifierCategory.Scourge,
        ModifierCategory.Veiled,
        ModifierCategory.Crucible,
    ];

    public static ModifierCategory GetModifierCategory(this string? apiId)
    {
        var value = apiId?.Split('.').First();
        return value.GetEnumFromValue<ModifierCategory>();
    }
}
