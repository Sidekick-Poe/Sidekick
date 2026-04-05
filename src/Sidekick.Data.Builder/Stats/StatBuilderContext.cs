using Sidekick.Data.Languages;
using Sidekick.Data.Stats;
using Sidekick.Data.StatsInvariant;
namespace Sidekick.Data.Builder.Stats;

public record StatBuilderContext(
    GameType Game,
    IGameLanguage Language,
    StatPatterns Patterns,
    List<TradeStatDefinition> TradeDefinitions,
    StatsInvariantDetails InvariantDetails);
