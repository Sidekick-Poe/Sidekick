namespace Sidekick.Modules.Trade.Poe2Scout;

public class DataSerie
{
    public required string Name { get; init; }

    public required List<DataPoint> Points { get; set; }
}
