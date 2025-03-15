namespace Sidekick.Apis.Poe.Trade.Results;

public class Result
{
    public required string Id { get; init; }
    public required Listing Listing { get; init; }
    public required ResultItem Item { get; init; }
}
