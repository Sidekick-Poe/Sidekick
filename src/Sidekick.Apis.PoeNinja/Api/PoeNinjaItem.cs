using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeNinja.Api
{
    internal record PoeNinjaItem
    {
        public string? BaseType { get; init; }

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

        [JsonPropertyName("levelRequired")]
        public int ItemLevel { get; init; }

        public double ChaosValue { get; init; }

        public string? DetailsId { get; init; }

        public string? Variant { get; init; }

        public int? ClusterSmallPassiveCount
        {
            get
            {
                if (Variant != null && Variant.EndsWith(" passives"))
                {
                    var intPart = Variant.Split(" ")[0];
                    if (int.TryParse(intPart, out var passiveCount))
                    {
                        return passiveCount;
                    }
                }

                return null;
            }
        }
    }
}
