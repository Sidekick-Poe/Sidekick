namespace Sidekick.Game.Scout;

public class ScoutItem
{
    public required int ItemId { get; set; }

    public string? CategoryApiId { get; set; }

    public string? Type { get; set; }

    public string? Text { get; set; }

    public string? Name { get; set; }

    public string? ApiId { get; set; }

    public string? BaseItemTypeId { get; set; }

    public string? IconUrl { get; set; }

    public bool IsCurrency { get; set; }

    public override string ToString() => Text ?? Name ?? Type ?? "";
}
