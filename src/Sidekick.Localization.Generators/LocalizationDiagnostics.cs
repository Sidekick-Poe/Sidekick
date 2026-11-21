using Microsoft.CodeAnalysis;

namespace Sidekick.Localization.Generators;

internal static class LocalizationDiagnostics
{
    private const string Category = "Localization";

    public static readonly DiagnosticDescriptor InvalidKey = new(
        id: "SKLOC001",
        title: "Resource key is not a valid C# identifier",
        messageFormat: "The key '{0}' in {1} is not a valid C# identifier, so it has no typed member",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConflictingKey = new(
        id: "SKLOC002",
        title: "Resource key conflicts with an IStringLocalizer member",
        messageFormat: "The key '{0}' in {1} has the same name as a member of IStringLocalizer, rename the key",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ObjectMemberKey = new(
        id: "SKLOC004",
        title: "Resource key has the name of an object method",
        messageFormat: "The key '{0}' in {1} has the same name as an object method, so it has no typed member and can only be read with the indexer",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnreadableFile = new(
        id: "SKLOC003",
        title: "Resource file could not be read",
        messageFormat: "{0} could not be read: {1}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    public static readonly DiagnosticDescriptor MissingEnumKeys = new(
        id: "SKLOC010",
        title: "Enum value has no resource key",
        messageFormat: "{0} has no key for these {1} values: {2}. Add the keys, or map the values to typed members in a switch.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UnverifiableKey = new(
        id: "SKLOC011",
        title: "Resource key can't be verified",
        messageFormat: "This resource key is computed at runtime and can't be verified, use the typed members instead (for example in a switch)",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor LiteralKey = new(
        id: "SKLOC012",
        title: "Use the typed resource member",
        messageFormat: "Use the typed member {0} instead of the key string",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingTranslation = new(
        id: "SKLOC013",
        title: "Resource has no translation",
        messageFormat: "{0} has no {1} translation",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    public static readonly DiagnosticDescriptor UnusedKey = new(
        id: "SKLOC015",
        title: "Resource key is not used",
        messageFormat: "The key '{0}' in {1} is not used, remove it from every language file",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    public static readonly DiagnosticDescriptor ForeignResources = new(
        id: "SKLOC016",
        title: "Resources of another project",
        messageFormat: "{0} belongs to another project, only {1} can be used from other projects: move the text to {1} or to this project's resources",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor OrphanTranslation = new(
        id: "SKLOC014",
        title: "Translated key is not in the base resource file",
        messageFormat: "The key '{0}' in {1} doesn't exist in {2}, remove it",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        customTags: WellKnownDiagnosticTags.CompilationEnd);
}
