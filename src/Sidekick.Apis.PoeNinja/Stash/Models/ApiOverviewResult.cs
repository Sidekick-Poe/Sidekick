namespace Sidekick.Apis.PoeNinja.Stash.Models;

public class ApiOverviewResult
{
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public List<ApiLine> Lines { get; set; } = [];
}