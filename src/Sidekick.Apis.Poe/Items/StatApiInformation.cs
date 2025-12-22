namespace Sidekick.Apis.Poe.Items;

public class StatApiInformation(string text)
{
    public string? Id { get; init; }

    public StatCategory Category { get; init; }

    public string Text { get; } = text;

    public override string ToString()
    {
        return $"[{Id}] {Text}";
    }
}
