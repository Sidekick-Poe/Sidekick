using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoePriceInfo.Api
{
    public record PriceInfoResult
    {
        public double? Min { get; init; }

        public double? Max { get; init; }

        public string? Currency { get; init; }

        [JsonPropertyName("pred_confidence_score")]
        public double ConfidenceScore { get; init; }

        [JsonPropertyName("warning_msg")]
        public string? WarningMessage { get; init; }

        [JsonPropertyName("error")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("error_msg")]
        public string? ErrorMessage { get; init; }
    }
}
