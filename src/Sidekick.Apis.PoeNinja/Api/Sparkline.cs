namespace Sidekick.Apis.PoeNinja.Api
{
    public record SparkLine
    {
        public double TotalChange { get; init; }

        public List<double?> Data { get; init; } = new();
    }
}
