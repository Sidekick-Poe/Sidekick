namespace Sidekick.Apis.PoeNinja.Api
{
    internal record PoeNinjaCurrency
    {
        public string? CurrencyTypeName { get; init; }

        public PoeNinjaExchange? Receive { get; init; }

        public SparkLine? ReceiveSparkLine { get; init; }

        public SparkLine? LowConfidenceReceiveSparkLine { get; init; }

        public string? DetailsId { get; init; }
    }
}
