using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Sidekick.Localization.Generators.Tests.GeneratorRunner;

namespace Sidekick.Localization.Generators.Tests;

/// <summary>
/// Runs the analyzer on in-memory sources and .resx files, the way the compiler does during a build.
/// </summary>
internal static class AnalyzerRunner
{
    public static async Task<List<Diagnostic>> Run(string source, params (string RelativePath, string Content)[] resxFiles)
    {
        var sourcePath = Path.Combine(ProjectDirectory, "Localization", "Source.cs");
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest), sourcePath)],
            GetReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        var additionalTexts = resxFiles
            .Select(x => (AdditionalText)new InMemoryAdditionalText(Path.Combine(ProjectDirectory, x.RelativePath), x.Content))
            .ToImmutableArray();

        var options = new AnalyzerOptions(additionalTexts, new TestOptionsProvider());
        var diagnostics = await compilation
            .WithAnalyzers([new LocalizationAnalyzer()], options)
            .GetAnalyzerDiagnosticsAsync();

        return diagnostics.ToList();
    }
}
