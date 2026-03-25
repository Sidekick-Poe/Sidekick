using Sidekick.Data;
namespace Sidekick.Apis.Poe.Tests.Poe1Korean;

public class Poe1KoreanFixture : ParserFixture
{
    protected override GameType GameType => GameType.PathOfExile1;
    protected override string Language => "ko";
}
