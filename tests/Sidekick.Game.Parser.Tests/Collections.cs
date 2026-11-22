using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Sidekick.Game.Parser.Tests;

public static class Collections
{
    public const string Poe1EnglishFixture = "Poe1-English";
    public const string Poe1KoreanFixture = "Poe1-Korean";

    public const string Poe2EnglishFixture = "Poe2-English";
}
