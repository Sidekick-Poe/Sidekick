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
        isEnabledByDefault: true);
}
