using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Localization;

namespace Sidekick.Localization.Generators.Tests;

/// <summary>
/// Runs the generator on in-memory sources and .resx files, the way the compiler does during a build.
/// </summary>
internal static class GeneratorRunner
{
    public static readonly string ProjectDirectory = Path.Combine(Path.GetTempPath(), "SidekickGeneratorTests") + Path.DirectorySeparatorChar;

    public const string RootNamespace = "Test.App";

    public const string SharedResources = "SharedResources";

    public static GeneratorResult Run(string source, params (string RelativePath, string Content)[] resxFiles)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Latest);
        var sourcePath = Path.Combine(ProjectDirectory, "Localization", "Source.cs");
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [CSharpSyntaxTree.ParseText(source, parseOptions, sourcePath)],
            GetReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        var additionalTexts = resxFiles
            .Select(x => (AdditionalText)new InMemoryAdditionalText(Path.Combine(ProjectDirectory, x.RelativePath), x.Content))
            .ToImmutableArray();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new LocalizerExtensionsGenerator().AsSourceGenerator()],
            additionalTexts,
            parseOptions,
            new TestOptionsProvider());

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiagnostics);

        var runResult = driver.GetRunResult();
        var generatedSources = runResult.Results.SelectMany(x => x.GeneratedSources).Select(x => x.SourceText.ToString()).ToList();
        var compileErrors = outputCompilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).ToList();
        var diagnostics = generatorDiagnostics.Concat(runResult.Diagnostics).Distinct().ToList();

        return new GeneratorResult(generatedSources, diagnostics, compileErrors);
    }

    public static string Resx(params (string Key, string Value)[] entries)
    {
        var data = string.Join("\n", entries.Select(x => $"""  <data name="{x.Key}" xml:space="preserve"><value>{x.Value}</value></data>"""));
        return $"""
                <?xml version="1.0" encoding="utf-8"?>
                <root>
                {data}
                </root>
                """;
    }

    private static IEnumerable<MetadataReference> GetReferences()
    {
        var platformAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty).Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        return platformAssemblies
            .Append(typeof(IStringLocalizer<>).Assembly.Location)
            .Distinct()
            .Select(x => MetadataReference.CreateFromFile(x));
    }

    private sealed class InMemoryAdditionalText(string path, string content) : AdditionalText
    {
        public override string Path => path;

        public override SourceText GetText(CancellationToken cancellationToken = default) => SourceText.From(content);
    }

    private sealed class TestOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private static readonly TestOptions Options = new(new Dictionary<string, string>
        {
            ["build_property.RootNamespace"] = RootNamespace,
            ["build_property.ProjectDir"] = ProjectDirectory,
            ["build_property.SharedLocalizationResources"] = SharedResources,
        });

        public override AnalyzerConfigOptions GlobalOptions => Options;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => TestOptions.Empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => TestOptions.Empty;
    }

    private sealed class TestOptions(Dictionary<string, string> values) : AnalyzerConfigOptions
    {
        public static readonly TestOptions Empty = new([]);

        public override bool TryGetValue(string key, out string value)
        {
            if (values.TryGetValue(key, out var found))
            {
                value = found;
                return true;
            }

            value = string.Empty;
            return false;
        }
    }
}

internal sealed record GeneratorResult(List<string> GeneratedSources, List<Diagnostic> Diagnostics, List<Diagnostic> CompileErrors);
