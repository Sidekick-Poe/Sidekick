namespace Sidekick.Modules.Wealth.Graphs;

public class DataPoint
{
    public required string DateString { get; init; }

    public decimal? Value { get; set; }
}
