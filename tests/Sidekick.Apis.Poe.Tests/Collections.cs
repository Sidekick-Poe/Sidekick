using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Sidekick.Apis.Poe.Tests;

public static class Collections
{
    public const string Poe1EnglishFixture = "Poe1EnglishFixture";
    public const string Poe1KoreanFixture = "Poe1KoreanFixture";

    public const string Poe2EnglishFixture = "Poe2EnglishFixture";
}
