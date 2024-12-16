using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Patterns
{
    public interface IParserPatterns : IInitializableService
    {
        Regex Crusader { get; }
        Regex Elder { get; }
        Regex Hunter { get; }
        Regex Requirements { get; }
        Dictionary<Rarity, Regex> Rarity { get; }
        Regex Redeemer { get; }
        Regex Shaper { get; }
        Regex Warlord { get; }
    }
}
