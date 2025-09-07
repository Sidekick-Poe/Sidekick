namespace Sidekick.Apis.Poe2Scout.Items.Models;

public class ApiItemsResult
{
    public int CurrentPage { get; set; }
    public int Pages { get; set; }
    public int Total { get; set; }
    public List<ScoutItem> Items { get; set; } = [];
}
