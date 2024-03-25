namespace Sidekick.Common.Game.Items;

public class Header
{
    public string? Name { get; init; }

    public string? Type { get; init; }

    public Class Class { get; init; }

    /// <inheritdoc />
    public override string? ToString()
    {
        if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Name))
        {
            return $"{Type} - {Name}";
        }

        return !string.IsNullOrEmpty(Type) ? Type : Name;
    }
}
