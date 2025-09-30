using Sidekick.Apis.Poe.Trade.Trade.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class PropertyFilter(PropertyDefinition definition)
{
    public bool ShowRow { get; init; } = true;

    public bool Checked { get; set; }

    public required string Text { get; init; }

    public string? Hint { get; init; }

    public PropertyDefinition Definition { get; } = definition;

    public LineContentType Type { get; init; } = LineContentType.Simple;
}
