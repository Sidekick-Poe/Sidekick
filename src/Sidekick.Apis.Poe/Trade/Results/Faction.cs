using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Results;

public class Faction
{
    public required string Id { get; init; }
    public required string Name { get; init; }

    public ModifierCategory Category => Id switch
    {
        "Faction1" => ModifierCategory.DruidsOfTheBrokenCircle,
        "Faction2" => ModifierCategory.BlackScytheMercenaries,
        "Faction3" => ModifierCategory.OrderOfTheChalice,
        "Faction4" => ModifierCategory.KnightsOfTheSun,
        _ => ModifierCategory.GrayText,
    };
}
