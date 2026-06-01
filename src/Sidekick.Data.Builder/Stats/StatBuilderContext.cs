using Sidekick.Data.Builder.Repoe.Models.Stats;
using Sidekick.Data.Builder.Trade.Models;
using Sidekick.Data.Languages;
using Sidekick.Data.StatsInvariant;
namespace Sidekick.Data.Builder.Stats;

public record StatBuilderContext(
    GameType Game,
    IGameLanguage Language,
    StatPatternBuilder PatternBuilder,
    List<RepoeStatTranslation> RepoeStats,
    List<TradeStatDefinition> TradeDefinitions,
    StatsInvariantDetails InvariantDetails);
