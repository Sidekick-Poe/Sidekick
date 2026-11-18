using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Sidekick.Localization.Generators;

/// <summary>
/// Checks what the typed members can't: keys built from enum values, keys computed at runtime, keys still written as strings, and missing translations.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LocalizationAnalyzer : DiagnosticAnalyzer
{
    private const string LocalizerMetadataName = "Microsoft.Extensions.Localization.IStringLocalizer`1";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        LocalizationDiagnostics.MissingEnumKeys,
        LocalizationDiagnostics.UnverifiableKey,
        LocalizationDiagnostics.LiteralKey,
        LocalizationDiagnostics.MissingTranslation,
        LocalizationDiagnostics.OrphanTranslation,
        LocalizationDiagnostics.UnreadableFile,
    ];

    public override void Initialize(AnalysisContext context)
    {
        // Razor components compile to generated code, so it has to be analyzed too.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(start =>
        {
            var localizerType = start.Compilation.GetTypeByMetadataName(LocalizerMetadataName);
            if (localizerType == null)
            {
                return;
            }

            var files = new Lazy<List<ResxFile>>(() => ReadBaseFiles(start.Options.AdditionalFiles, start.CancellationToken));
            start.RegisterOperationAction(operation => AnalyzeIndexer(operation, localizerType, files), OperationKind.PropertyReference);
        });

        context.RegisterCompilationAction(AnalyzeTranslations);
    }

    private static List<ResxFile> ReadBaseFiles(ImmutableArray<AdditionalText> additionalFiles, CancellationToken cancellationToken)
    {
        return additionalFiles
            .Where(x => ResxFile.IsBase(x.Path))
            .Select(x => ResxFile.Parse(x.Path, x.GetText(cancellationToken)?.ToString()))
            .ToList();
    }

    private static void AnalyzeIndexer(OperationAnalysisContext context, INamedTypeSymbol localizerType, Lazy<List<ResxFile>> files)
    {
        var operation = (IPropertyReferenceOperation)context.Operation;
        if (!operation.Property.IsIndexer || operation.Arguments.Length == 0)
        {
            return;
        }

        if (operation.Instance?.Type is not INamedTypeSymbol { IsGenericType: true } instanceType
            || !SymbolEqualityComparer.Default.Equals(instanceType.OriginalDefinition, localizerType))
        {
            return;
        }

        // The typed members are generated as localizer["Key"], they are the one place where the key string belongs.
        if (operation.Syntax.SyntaxTree.FilePath.EndsWith(".Localization.g.cs", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var resourcesType = instanceType.TypeArguments[0];
        var key = operation.Arguments[0].Value;
        if (key.ConstantValue is { HasValue: true, Value: string literal })
        {
            // Keys without a typed member (Equals, invalid identifiers) can only be read with the indexer.
            if (LocalizerExtensionsGenerator.HasTypedMember(literal))
            {
                var member = $"{GetReceiverName(operation.Instance)}.{literal}{(operation.Arguments.Length > 1 ? "(...)" : string.Empty)}";
                context.ReportDiagnostic(Diagnostic.Create(LocalizationDiagnostics.LiteralKey, key.Syntax.GetLocation(), member));
            }

            return;
        }

        var enumValue = GetEnumToStringInstance(key);
        var file = enumValue != null ? FindFile(files.Value, resourcesType) : null;
        if (enumValue == null || file == null)
        {
            context.ReportDiagnostic(Diagnostic.Create(LocalizationDiagnostics.UnverifiableKey, key.Syntax.GetLocation()));
            return;
        }

        var keys = new HashSet<string>(file.Entries.Select(x => x.Key));
        var handled = GetValuesHandledBeforeDiscard(operation, enumValue);
        var missing = ((INamedTypeSymbol)enumValue.Type!).GetMembers()
            .OfType<IFieldSymbol>()
            .Where(x => x.HasConstantValue && !handled.Contains(x.ConstantValue) && !keys.Contains(x.Name))
            .Select(x => x.Name)
            .ToList();

        if (missing.Count > 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(LocalizationDiagnostics.MissingEnumKeys, key.Syntax.GetLocation(), Path.GetFileName(file.Path), enumValue.Type!.Name, string.Join(", ", missing)));
        }
    }

    private static string GetReceiverName(IOperation? instance)
    {
        return instance?.Syntax.ToString() ?? "Resources";
    }

    /// <summary>
    /// For a key written value.ToString() where value is an enum, gives the operation of value.
    /// </summary>
    private static IOperation? GetEnumToStringInstance(IOperation key)
    {
        if (key is not IInvocationOperation { TargetMethod.Name: "ToString", Arguments.Length: 0, Instance: { } instance })
        {
            return null;
        }

        var value = Unwrap(instance);
        return value.Type?.TypeKind == TypeKind.Enum ? value : null;
    }

    private static IOperation Unwrap(IOperation operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
    }

    /// <summary>
    /// When the lookup is the discard arm of a switch on the same value (_ => Resources[value.ToString()]), the values matched by the arms above never reach it.
    /// </summary>
    private static HashSet<object?> GetValuesHandledBeforeDiscard(IOperation lookup, IOperation enumValue)
    {
        var handled = new HashSet<object?>();
        var arm = lookup.Parent;
        while (arm != null && arm is not ISwitchExpressionArmOperation)
        {
            arm = arm.Parent;
        }

        if (arm is not ISwitchExpressionArmOperation { Pattern: IDiscardPatternOperation, Guard: null } discardArm
            || discardArm.Parent is not ISwitchExpressionOperation switchExpression
            || !IsSameValue(Unwrap(switchExpression.Value), enumValue))
        {
            return handled;
        }

        foreach (var previous in switchExpression.Arms.TakeWhile(x => x != discardArm).Where(x => x.Guard == null))
        {
            AddPatternValues(previous.Pattern, handled);
        }

        return handled;
    }

    private static void AddPatternValues(IPatternOperation pattern, HashSet<object?> values)
    {
        switch (pattern)
        {
            case IConstantPatternOperation { Value.ConstantValue.HasValue: true } constant:
                values.Add(constant.Value.ConstantValue.Value);
                break;

            case IBinaryPatternOperation { OperatorKind: BinaryOperatorKind.Or } or:
                AddPatternValues(or.LeftPattern, values);
                AddPatternValues(or.RightPattern, values);
                break;
        }
    }

    private static bool IsSameValue(IOperation left, IOperation right)
    {
        var leftSymbol = GetReferencedSymbol(left);
        return leftSymbol != null && SymbolEqualityComparer.Default.Equals(leftSymbol, GetReferencedSymbol(right));
    }

    private static ISymbol? GetReferencedSymbol(IOperation operation)
    {
        return operation switch
        {
            ILocalReferenceOperation local => local.Local,
            IParameterReferenceOperation parameter => parameter.Parameter,
            IFieldReferenceOperation field => field.Field,
            IPropertyReferenceOperation property => property.Property,
            _ => null,
        };
    }

    private static ResxFile? FindFile(List<ResxFile> files, ITypeSymbol resourcesType)
    {
        var candidates = files.Where(x => x.ClassName == resourcesType.Name).ToList();
        if (candidates.Count <= 1)
        {
            return candidates.FirstOrDefault();
        }

        var directories = resourcesType.DeclaringSyntaxReferences.Select(x => Path.GetDirectoryName(x.SyntaxTree.FilePath)).ToList();
        return candidates.FirstOrDefault(x => directories.Any(directory => string.Equals(Path.GetDirectoryName(x.Path), directory, StringComparison.OrdinalIgnoreCase)));
    }

    private static string NormalizePath(string path) => Path.GetFullPath(path);

    private static void AnalyzeTranslations(CompilationAnalysisContext context)
    {
        context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.LocalizationCultures", out var cultureList);
        var cultures = (cultureList ?? string.Empty).Split([';', ','], StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

        // A file can be listed twice in AdditionalFiles (an explicit item on top of the glob), so keep the first one.
        var files = context.Options.AdditionalFiles
            .Where(x => x.Path.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
            .GroupBy(x => NormalizePath(x.Path), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => ResxFile.Parse(x.First().Path, x.First().GetText(context.CancellationToken)?.ToString()), StringComparer.OrdinalIgnoreCase);

        // The generator reports base files it can't read, translations are only read here.
        foreach (var unreadable in files.Values.Where(x => x.Error != null && !ResxFile.IsBase(x.Path)))
        {
            context.ReportDiagnostic(Diagnostic.Create(LocalizationDiagnostics.UnreadableFile, unreadable.GetFileLocation(), Path.GetFileName(unreadable.Path), unreadable.Error));
        }

        foreach (var baseFile in files.Values.Where(x => ResxFile.IsBase(x.Path) && x.Error == null && x.Entries.Any()))
        {
            var baseName = Path.GetFileName(baseFile.Path);
            foreach (var culture in cultures)
            {
                var translationPath = Path.Combine(Path.GetDirectoryName(baseFile.Path) ?? string.Empty, $"{baseFile.ClassName}.{culture}.resx");
                if (!files.TryGetValue(NormalizePath(translationPath), out var translation))
                {
                    context.ReportDiagnostic(Diagnostic.Create(LocalizationDiagnostics.MissingTranslation, baseFile.GetFileLocation(), baseName, culture));
                    continue;
                }

                if (translation.Error != null)
                {
                    continue;
                }

                var translatedKeys = new HashSet<string>(translation.Entries.Select(x => x.Key));
                foreach (var entry in baseFile.Entries.Where(x => !translatedKeys.Contains(x.Key)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(LocalizationDiagnostics.MissingTranslation, baseFile.GetLocation(entry), $"The key '{entry.Key}'", culture));
                }
            }
        }

        foreach (var translation in files.Values)
        {
            if (!ResxFile.TryGetTranslation(translation.Path, out var basePath, out _) || !files.TryGetValue(NormalizePath(basePath), out var baseFile) || baseFile.Error != null)
            {
                continue;
            }

            var baseKeys = new HashSet<string>(baseFile.Entries.Select(x => x.Key));
            foreach (var entry in translation.Entries.Where(x => !baseKeys.Contains(x.Key)))
            {
                context.ReportDiagnostic(Diagnostic.Create(LocalizationDiagnostics.OrphanTranslation, translation.GetLocation(entry), entry.Key, Path.GetFileName(translation.Path), Path.GetFileName(basePath)));
            }
        }
    }
}
