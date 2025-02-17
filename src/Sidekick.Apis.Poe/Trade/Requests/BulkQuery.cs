namespace Sidekick.Apis.Poe.Trade.Requests;

public class BulkQuery
{
    public List<string> Have { get; } = new();

    public List<string> Want { get; } = new();

    public int Minimum { get; set; } = 5;

    public Status Status { get; } = new();
}
