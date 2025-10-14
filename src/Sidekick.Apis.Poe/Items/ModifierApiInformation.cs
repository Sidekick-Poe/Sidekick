namespace Sidekick.Apis.Poe.Items;

public class ModifierApiInformation(string apiText)
{
    public string? ApiId { get; init; }

    public ModifierCategory Category { get; init; }

    public string ApiText { get; } = apiText;

    public override string ToString()
    {
        return $"[{ApiId}] {ApiText}";
    }
}
