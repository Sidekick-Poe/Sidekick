using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeNinja.Api
{
    internal record PoeNinjaItem
    {
        public string? Name { get; init; }

        public int MapTier { get; init; }

        public int Links { get; init; }

        public int ItemClass { get; init; }

        [JsonPropertyName("Sparkline")]
        public SparkLine? SparkLine { get; init; }

        [JsonPropertyName("lowConfidenceSparkline")]
        public SparkLine? LowConfidenceSparkLine { get; init; }

        public bool Corrupted { get; init; }

        public int GemLevel { get; init; }

        public int GemQuality { get; init; }

        public double ChaosValue { get; init; }

        public string? DetailsId { get; init; }
    }
}
