using Sidekick.Data.Items.Models;
namespace Sidekick.Apis.Poe.Tests.Poe1English;

public class Poe1EnglishFixture : ParserFixture
{
    protected override GameType GameType => GameType.PathOfExile1;
    protected override string Language => "en";
}
