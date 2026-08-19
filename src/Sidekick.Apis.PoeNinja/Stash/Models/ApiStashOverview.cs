namespace Sidekick.Apis.PoeNinja.Stash.Models;

public class ApiStashOverview
{
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public List<ApiStashLine> Lines { get; set; } = [];
}