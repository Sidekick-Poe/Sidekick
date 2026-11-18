using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using static Sidekick.Localization.Generators.Tests.GeneratorRunner;

namespace Sidekick.Localization.Generators.Tests;

public class LocalizationAnalyzerTests
{
    private const string Source = """
                                  using Microsoft.Extensions.Localization;

                                  namespace Test.App.Localization;

                                  public class TestResources
                                  {
                                  }

                                  public enum Color
                                  {
                                      Red,
                                      Green,
                                      Blue,
                                  }

                                  public class Usage(IStringLocalizer<TestResources> resources)
                                  {
                                  """;

    private static string WithMember(string member) => Source + "\n    " + member + "\n}\n";

    [Fact]
    public async Task LiteralKeyIsReported()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember("public string Text => resources[\"Title\"];"),
            ("Localization/TestResources.resx", Resx(("Title", "Title"))));

        var diagnostic = Assert.Single(diagnostics, x => x.Id == "SKLOC012");
        Assert.Contains("resources.Title", diagnostic.GetMessage());
    }

    [Fact]
    public async Task LiteralKeyWithArgumentsPointsToTheMethod()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember("public string Text => resources[\"Count\", 1];"),
            ("Localization/TestResources.resx", Resx(("Count", "{0} items"))));

        Assert.Contains("resources.Count(...)", Assert.Single(diagnostics, x => x.Id == "SKLOC012").GetMessage());
    }

    [Fact]
    public async Task LiteralKeyWithoutTypedMemberIsNotReported()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember("public string Text => resources[\"Equals\"] + resources[\"Not-Valid\"];"),
            ("Localization/TestResources.resx", Resx(("Equals", "Equals"), ("Not-Valid", "Text"))));

        Assert.DoesNotContain(diagnostics, x => x.Id == "SKLOC012");
    }

    [Fact]
    public async Task EnumKeyWithoutResourceFileIsUnverifiable()
    {
        var diagnostics = await AnalyzerRunner.Run(WithMember("public string Text(Color color) => resources[color.ToString()];"));

        Assert.Single(diagnostics, x => x.Id == "SKLOC011");
    }

    [Fact]
    public async Task UnreadableTranslationIsReportedAsUnreadable()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember(string.Empty),
            ("Localization/TestResources.resx", Resx(("Title", "Title"))),
            ("Localization/TestResources.fr.resx", "<root><data"));

        Assert.Single(diagnostics, x => x.Id == "SKLOC003");
        Assert.DoesNotContain(diagnostics, x => x.Id == "SKLOC013");
    }

    [Fact]
    public async Task MissingEnumKeyIsReported()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember("public string Text(Color color) => resources[color.ToString()];"),
            ("Localization/TestResources.resx", Resx(("Red", "Red"), ("Green", "Green"))));

        var diagnostic = Assert.Single(diagnostics, x => x.Id == "SKLOC010");
        Assert.Contains("values: Blue.", diagnostic.GetMessage());
    }

    [Fact]
    public async Task EnumWithAllKeysIsNotReported()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember("public string Text(Color color) => resources[color.ToString()];"),
            ("Localization/TestResources.resx", Resx(("Red", "Red"), ("Green", "Green"), ("Blue", "Blue"))));

        Assert.DoesNotContain(diagnostics, x => x.Id == "SKLOC010");
    }

    [Fact]
    public async Task ValuesHandledBeforeTheDiscardArmAreNotReported()
    {
        const string member = """
                              public string Text(Color color) => color switch
                                  {
                                      Color.Red or Color.Green => "Warm",
                                      _ => resources[color.ToString()],
                                  };
                              """;

        var diagnostics = await AnalyzerRunner.Run(WithMember(member), ("Localization/TestResources.resx", Resx(("Blue", "Blue"))));

        Assert.DoesNotContain(diagnostics, x => x.Id == "SKLOC010");
    }

    [Fact]
    public async Task ComputedKeyIsReported()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember("public string Text(string key) => resources[key];"),
            ("Localization/TestResources.resx", Resx(("Title", "Title"))));

        Assert.Single(diagnostics, x => x.Id == "SKLOC011");
    }

    [Fact]
    public async Task MissingTranslationsAreReported()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember(string.Empty),
            ("Localization/TestResources.resx", Resx(("Title", "Title"), ("Close", "Close"))),
            ("Localization/TestResources.fr.resx", Resx(("Title", "Titre"))),
            ("Localization/OtherResources.resx", Resx(("Title", "Title"))));

        var missing = diagnostics.Where(x => x.Id == "SKLOC013").Select(x => x.GetMessage()).ToList();
        Assert.Equal(2, missing.Count);
        Assert.Contains("The key 'Close' has no fr translation", missing);
        Assert.Contains("OtherResources.resx has no fr translation", missing);
    }

    [Fact]
    public async Task UnusedKeyIsReported()
    {
        const string member = """
                              public string Title => resources.Title;
                              public System.Func<object, string> Count => resources.Count;
                              public string Literal => resources["Equals"];
                              """;

        var diagnostics = await AnalyzerRunner.Run(
            WithMember(member),
            ("Localization/TestResources.resx", Resx(("Title", "Title"), ("Count", "{0} items"), ("Equals", "Equals"), ("Unused", "Unused"))));

        var diagnostic = Assert.Single(diagnostics, x => x.Id == "SKLOC015");
        Assert.Contains("'Unused'", diagnostic.GetMessage());
    }

    [Fact]
    public async Task EnumValuesCountAsUsed()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember("public string Text(Color color) => resources[color.ToString()];"),
            ("Localization/TestResources.resx", Resx(("Red", "Red"), ("Green", "Green"), ("Blue", "Blue"))));

        Assert.DoesNotContain(diagnostics, x => x.Id == "SKLOC015");
    }

    [Fact]
    public async Task UnusedKeysAreNotReportedForComputedLookups()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember("public string Text(string key) => resources[key];"),
            ("Localization/TestResources.resx", Resx(("Unused", "Unused"))));

        Assert.DoesNotContain(diagnostics, x => x.Id == "SKLOC015");
    }

    [Fact]
    public async Task UnusedKeysAreNotReportedForSharedResources()
    {
        const string source = """
                              namespace Test.App.Localization;

                              public class SharedResources
                              {
                              }
                              """;

        var diagnostics = await AnalyzerRunner.Run(source, ("Localization/SharedResources.resx", Resx(("Unused", "Unused"))));

        Assert.DoesNotContain(diagnostics, x => x.Id == "SKLOC015");
    }

    [Fact]
    public async Task GetStringIsAnalyzedLikeTheIndexer()
    {
        const string member = """
                              public string Literal => resources.GetString("Title");
                              public string Computed(string key) => resources.GetString(key);
                              """;

        var diagnostics = await AnalyzerRunner.Run(WithMember(member), ("Localization/TestResources.resx", Resx(("Title", "Title"))));

        Assert.Single(diagnostics, x => x.Id == "SKLOC012");
        Assert.Single(diagnostics, x => x.Id == "SKLOC011");
    }

    [Fact]
    public async Task EnumValuesHandledBeforeTheDiscardArmAreNotUsed()
    {
        const string member = """
                              public string Text(Color color) => color switch
                                  {
                                      Color.Red => "Warm",
                                      _ => resources[color.ToString()],
                                  };
                              """;

        var diagnostics = await AnalyzerRunner.Run(
            WithMember(member),
            ("Localization/TestResources.resx", Resx(("Red", "Red"), ("Green", "Green"), ("Blue", "Blue"))));

        Assert.Contains("'Red'", Assert.Single(diagnostics, x => x.Id == "SKLOC015").GetMessage());
    }

    [Fact]
    public async Task ResourcesOfAnotherProjectAreReported()
    {
        var otherProject = CSharpCompilation.Create(
                "OtherProject",
                [CSharpSyntaxTree.ParseText("namespace Other; public class OtherResources { } public class SharedResources { }")],
                GetReferences(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .ToMetadataReference();

        const string source = """
                              using Microsoft.Extensions.Localization;

                              namespace Test.App;

                              public class Usage(IStringLocalizer<Other.OtherResources> other, IStringLocalizer<Other.SharedResources> shared)
                              {
                                  public string Other => other["Title"];
                                  public string Shared => shared["Title"];
                              }
                              """;

        var diagnostics = await AnalyzerRunner.Run(source, [otherProject]);

        Assert.Contains("OtherResources", Assert.Single(diagnostics, x => x.Id == "SKLOC016").GetMessage());
    }

    [Fact]
    public async Task OrphanTranslationIsReported()
    {
        var diagnostics = await AnalyzerRunner.Run(
            WithMember(string.Empty),
            ("Localization/TestResources.resx", Resx(("Title", "Title"))),
            ("Localization/TestResources.fr.resx", Resx(("Title", "Titre"), ("Removed", "Supprimé"))));

        var diagnostic = Assert.Single(diagnostics, x => x.Id == "SKLOC014");
        Assert.Contains("'Removed'", diagnostic.GetMessage());
    }
}
