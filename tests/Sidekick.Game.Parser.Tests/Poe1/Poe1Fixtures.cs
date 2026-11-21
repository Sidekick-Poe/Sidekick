namespace Sidekick.Game.Parser.Tests.Poe1;

public class Poe1EnglishFixture : ParserFixture
{
    protected override GameType GameType => GameType.Poe1;
    protected override string Language => "en";
}

public class Poe1KoreanFixture : ParserFixture
{
    protected override GameType GameType => GameType.Poe1;
    protected override string Language => "ko";
}
