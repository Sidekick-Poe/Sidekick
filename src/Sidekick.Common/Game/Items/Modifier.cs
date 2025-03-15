namespace Sidekick.Common.Game.Items;

public class Modifier(string text)
{
    public string? ApiId { get; init; }

    public string? Tier { get; init; }

    public string? TierName { get; set; }

    public ModifierCategory Category { get; init; }

    public string Text { get; } = text;

    public override string ToString()
    {
        return $"[{ApiId}] {Text}";
    }
}
