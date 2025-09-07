namespace Sidekick.Apis.Poe2Scout.Items.Models;

public class ScoutItem
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int CurrencyCategoryId { get; set; }
    public string? ApiId { get; set; }
    public string? Text { get; set; }
    public string? Name { get; set; }
    public string? CategoryApiId { get; set; }
    public string? IconUrl { get; set; }
    public bool IsChanceable { get; set; }

    public ScoutItemMetadata? ItemMetadata { get; set; }
}
