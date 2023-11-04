namespace Sidekick.Common.Game.Items.AdditionalInformation
{
    public record ClusterJewelInformation
    {
        public required List<string> GrantTexts { get; init; }

        public required int SmallPassiveCount { get; init; }

        public required int ItemLevel { get; init; }
    }
}
