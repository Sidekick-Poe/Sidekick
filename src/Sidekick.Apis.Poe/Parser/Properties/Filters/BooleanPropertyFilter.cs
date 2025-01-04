namespace Sidekick.Apis.Poe.Parser.Properties.Filters;

public class BooleanPropertyFilter(PropertyDefinition definition)
{
    public required bool ShowCheckbox { get; init; }

    public bool ShowRow { get; init; } = true;

    public bool Checked { get; set; }

    public required string Text { get; init; }

    public PropertyDefinition Definition { get; } = definition;
}
