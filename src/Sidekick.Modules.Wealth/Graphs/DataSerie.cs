namespace Sidekick.Modules.Wealth.Graphs;

public class DataSerie
{
    public required string Name { get; init; }

    public required List<DataPoint> Points { get; set; }
}
