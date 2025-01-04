namespace Sidekick.Apis.Poe.Parser.Properties.Filters;

public class BooleanPropertyFilter(PropertyDefinition definition)
{
    /// <summary>
    /// Represents a key used to uniquely identify a property filter to do custom work on this filter.
    /// </summary>
    public string? IndentifyingKey { get; init; }

    public required bool ShowCheckbox { get; init; }

    public bool ShowRow { get; init; } = true;

    public bool Checked { get; set; }

    public required string Text { get; init; }

    public PropertyDefinition Definition { get; } = definition;
}
