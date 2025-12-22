namespace Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

public class StatFilters
{
    public string? Id { get; set; }
    public StatFilterValue? Value { get; set; }
    public static bool Disabled => false;
}
