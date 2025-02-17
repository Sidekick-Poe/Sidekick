namespace Sidekick.Apis.Poe.Bulk.Results;

public class BulkResponse
{
    public string? Id { get; set; }

    public int Total { get; set; }

    public Dictionary<string, BulkResult>? Result { get; set; }
}
