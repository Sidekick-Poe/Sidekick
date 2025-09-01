using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Headers;

public interface IRarityParser : IInitializableService
{
    Rarity Parse(ParsingItem parsingItem);
}
