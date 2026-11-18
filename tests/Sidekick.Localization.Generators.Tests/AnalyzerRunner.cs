using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Sidekick.Localization.Generators.Tests.GeneratorRunner;

namespace Sidekick.Localization.Generators.Tests;

/// <summary>
/// Runs the generator, then the analyzer, on in-memory sources and .resx files, the way the compiler does during a build.
/// </summary>
internal static class AnalyzerRunner
{
    public static Task<List<Diagnostic>> Run(string source, params (string RelativePath, string Content)[] resxFiles) => Run(source, [], resxFiles);

    public static async Task<List<Diagnostic>> Run(string source, MetadataReference[] references, params (string RelativePath, string Content)[] resxFiles)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Latest);
        var sourcePath = Path.Combine(ProjectDirectory, "Localization", "Source.cs");
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [CSharpSyntaxTree.ParseText(source, parseOptions, sourcePath)],
            GetReferences().Concat(references),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        var additionalTexts = resxFiles
            .Select(x => (AdditionalText)new InMemoryAdditionalText(Path.Combine(ProjectDirectory, x.RelativePath), x.Content))
            .ToImmutableArray();

        var optionsProvider = new TestOptionsProvider();
        CSharpGeneratorDriver
            .Create([new LocalizerExtensionsGenerator().AsSourceGenerator()], additionalTexts, parseOptions, optionsProvider)
            .RunGeneratorsAndUpdateCompilation(compilation, out var generatedCompilation, out _);

        var diagnostics = await generatedCompilation
            .WithAnalyzers([new LocalizationAnalyzer()], new AnalyzerOptions(additionalTexts, optionsProvider))
            .GetAnalyzerDiagnosticsAsync();

        return diagnostics.ToList();
    }
}
