using System.Text.RegularExpressions;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Patterns
{
    public interface IParserPatterns : IInitializableService
    {
        Regex Crusader { get; }
        Regex Elder { get; }
        Regex Hunter { get; }
        Regex Requirements { get; }
        Regex Redeemer { get; }
        Regex Shaper { get; }
        Regex Warlord { get; }
    }
}
