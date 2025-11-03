namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class ApiOverviewResult
{
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public ApiCore? Core { get; set; }

    public List<ApiItem> Items { get; set; } = [];

    public List<ApiLine> Lines { get; set; } = [];
}