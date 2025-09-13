namespace Sidekick.Common.Game.Items.AdditionalInformation;

public record ClusterJewelInformation
{
    public required string GrantText { get; init; }

    public required int SmallPassiveCount { get; init; }
}
