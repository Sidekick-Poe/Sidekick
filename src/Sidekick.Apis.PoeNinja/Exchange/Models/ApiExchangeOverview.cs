namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class ApiExchangeOverview
{
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public ApiExchangeCore? Core { get; set; }

    public List<ApiExchangeItem> Items { get; set; } = [];

    public List<ApiExchangeLine> Lines { get; set; } = [];
}