using Sidekick.Data.Items.Models;
namespace Sidekick.Apis.Poe.Tests.Poe2English;

public class Poe2EnglishFixture : ParserFixture
{
    protected override GameType GameType => GameType.PathOfExile2;
    protected override string Language => "en";
}
