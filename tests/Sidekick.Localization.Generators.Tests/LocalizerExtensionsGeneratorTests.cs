using Xunit;
using static Sidekick.Localization.Generators.Tests.GeneratorRunner;

namespace Sidekick.Localization.Generators.Tests;

public class LocalizerExtensionsGeneratorTests
{
    private const string MarkerClass = """
                                       namespace Test.App.Localization;

                                       public class TestResources
                                       {
                                       }
                                       """;

    [Fact]
    public void KeysBecomeProperties()
    {
        var source = MarkerClass + """

                                   public class Usage(Microsoft.Extensions.Localization.IStringLocalizer<TestResources> resources)
                                   {
                                       public string Title => resources.Open_Website;
                                   }
                                   """;

        var result = Run(source, ("Localization/TestResources.resx", Resx(("Open_Website", "Open Website"))));

        Assert.Empty(result.CompileErrors);
        Assert.Contains("public string Open_Website => localizer[\"Open_Website\"];", Assert.Single(result.GeneratedSources));
    }

    [Fact]
    public void MissingKeyDoesNotCompile()
    {
        var source = MarkerClass + """

                                   public class Usage(Microsoft.Extensions.Localization.IStringLocalizer<TestResources> resources)
                                   {
                                       public string Title => resources.Open_Websit;
                                   }
                                   """;

        var result = Run(source, ("Localization/TestResources.resx", Resx(("Open_Website", "Open Website"))));

        Assert.Contains(result.CompileErrors, x => x.Id == "CS1061");
    }

    [Fact]
    public void PlaceholdersBecomeMethodArguments()
    {
        var source = MarkerClass + """

                                   public class Usage(Microsoft.Extensions.Localization.IStringLocalizer<TestResources> resources)
                                   {
                                       public string Text => resources.Range(1, 5);
                                   }
                                   """;

        var result = Run(
            source,
            ("Localization/TestResources.resx", Resx(("Range", "{0} to {1:N0}"), ("Braces", "{{0}} is literal"))));

        Assert.Empty(result.CompileErrors);
        var generated = Assert.Single(result.GeneratedSources);
        Assert.Contains("public string Range(object arg0, object arg1) => localizer[\"Range\", arg0, arg1];", generated);
        Assert.Contains("public string Braces => localizer[\"Braces\"];", generated);
    }

    [Fact]
    public void ProjectResourcesAreInternal()
    {
        var result = Run(MarkerClass, ("Localization/TestResources.resx", Resx(("Title", "Title"))));

        Assert.Contains("internal static class TestResourcesLocalizerExtensions", Assert.Single(result.GeneratedSources));
    }

    [Fact]
    public void SharedResourcesArePublic()
    {
        const string source = """
                              namespace Test.App.Localization;

                              public class SharedResources
                              {
                              }
                              """;

        var result = Run(source, ("Localization/SharedResources.resx", Resx(("Close", "Close"))));

        Assert.Contains("public static class SharedResourcesLocalizerExtensions", Assert.Single(result.GeneratedSources));
    }

    [Fact]
    public void TranslationsAreIgnored()
    {
        var result = Run(
            MarkerClass,
            ("Localization/TestResources.resx", Resx(("Title", "Title"))),
            ("Localization/TestResources.fr.resx", Resx(("Title", "Titre"), ("Only_In_French", "Seulement en français"))));

        var generated = Assert.Single(result.GeneratedSources);
        Assert.DoesNotContain("Only_In_French", generated);
    }

    [Fact]
    public void NonStringEntriesAreIgnored()
    {
        const string resx = """
                            <?xml version="1.0" encoding="utf-8"?>
                            <root>
                              <data name="Title"><value>Title</value></data>
                              <data name="Color1" type="System.Drawing.Color, System.Drawing"><value>Blue</value></data>
                            </root>
                            """;

        var result = Run(MarkerClass, ("Localization/TestResources.resx", resx));

        Assert.DoesNotContain("Color1", Assert.Single(result.GeneratedSources));
    }

    [Fact]
    public void KeywordKeysAreEscaped()
    {
        var result = Run(MarkerClass, ("Localization/TestResources.resx", Resx(("class", "Class"))));

        Assert.Empty(result.CompileErrors);
        Assert.Contains("public string @class => localizer[\"class\"];", Assert.Single(result.GeneratedSources));
    }

    [Fact]
    public void InvalidKeyIsReported()
    {
        var result = Run(MarkerClass, ("Localization/TestResources.resx", Resx(("Not-Valid", "Text"))));

        Assert.Empty(result.CompileErrors);
        Assert.Contains(result.Diagnostics, x => x.Id == "SKLOC001");
    }

    [Fact]
    public void ConflictingKeyIsReported()
    {
        var result = Run(MarkerClass, ("Localization/TestResources.resx", Resx(("GetAllStrings", "Text"))));

        Assert.Contains(result.Diagnostics, x => x.Id == "SKLOC002");
    }

    [Fact]
    public void ObjectMemberKeyHasNoMember()
    {
        var result = Run(MarkerClass, ("Localization/TestResources.resx", Resx(("Equals", "Equals"), ("Title", "Title"))));

        Assert.Empty(result.CompileErrors);
        Assert.DoesNotContain("Equals", Assert.Single(result.GeneratedSources));
        Assert.Contains(result.Diagnostics, x => x.Id == "SKLOC004" && x.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Info);
    }

    [Fact]
    public void TypeInAnotherFolderIsNotUsed()
    {
        var result = Run(MarkerClass, ("Other/TestResources.resx", Resx(("Title", "Title"))));

        Assert.Contains($"namespace {RootNamespace}.Other;", Assert.Single(result.GeneratedSources));
    }

    [Fact]
    public void FolderNamespaceIsUsedWhenTheTypeIsNotVisible()
    {
        var result = Run("namespace Test.App;", ("Setup/Setup.resx", Resx(("Title", "Setup"))));

        var generated = Assert.Single(result.GeneratedSources);
        Assert.Contains($"namespace {RootNamespace}.Setup;", generated);
        Assert.Contains($"IStringLocalizer<global::{RootNamespace}.Setup.Setup>", generated);
    }
}
