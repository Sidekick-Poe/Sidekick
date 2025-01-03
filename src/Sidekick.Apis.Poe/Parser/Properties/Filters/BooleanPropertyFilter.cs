namespace Sidekick.Apis.Poe.Parser.Properties.Filters;

public class BooleanPropertyFilter
{
    internal BooleanPropertyFilter(PropertyDefinition definition)
    {
        Definition = definition;
    }

    public required bool ShowCheckbox { get; init; }

    public bool Checked { get; set; }

    public required string Text { get; init; }

    internal PropertyDefinition Definition { get; }
}
