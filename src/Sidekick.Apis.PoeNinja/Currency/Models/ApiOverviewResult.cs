namespace Sidekick.Apis.PoeNinja.Currency.Models;

public class ApiOverviewResult
{
    public ApiCore Core { get; set; }

    public List<ApiItem> Items { get; set; }

    public List<ApiLine> Lines { get; set; }
}