using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe1;

// This file has no code, and is never created. Its purpose is simply
// to be the place to apply [CollectionDefinition] and all the
// ICollectionFixture<> interfaces.

[CollectionDefinition(Collections.Poe1EnglishFixture)]
public class Poe1EnglishCollection : ICollectionFixture<Poe1EnglishFixture> {}

[CollectionDefinition(Collections.Poe1KoreanFixture)]
public class Poe1KoreanCollection : ICollectionFixture<Poe1KoreanFixture> {}
