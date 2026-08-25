using Sidekick.Game;
namespace Sidekick.Apis.Poe.Tests.Poe1Korean;

public class Poe1KoreanFixture : ParserFixture
{
    protected override GameType GameType => GameType.Poe1;
    protected override string Language => "ko";
}
