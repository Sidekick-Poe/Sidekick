using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Trade.Items.Models;

public class LogbookFaction
{
    public required string Id { get; init; }
    public required string Name { get; init; }

    public StatCategory Category => Id switch
    {
        "Faction1" => StatCategory.DruidsOfTheBrokenCircle,
        "Faction2" => StatCategory.BlackScytheMercenaries,
        "Faction3" => StatCategory.OrderOfTheChalice,
        "Faction4" => StatCategory.KnightsOfTheSun,
        _ => StatCategory.GrayText,
    };
}
