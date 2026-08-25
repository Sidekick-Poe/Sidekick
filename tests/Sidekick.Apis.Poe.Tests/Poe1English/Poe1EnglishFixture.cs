using Sidekick.Game;
namespace Sidekick.Apis.Poe.Tests.Poe1English;

public class Poe1EnglishFixture : ParserFixture
{
    protected override GameType GameType => GameType.Poe1;
    protected override string Language => "en";
}
