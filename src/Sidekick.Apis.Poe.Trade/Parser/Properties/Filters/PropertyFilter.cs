using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Apis.Poe.Trade.Parser.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;

public class PropertyFilter(PropertyDefinition definition) : IAutoSelectableFilter
{
    public bool ShowRow { get; init; } = true;

    public bool? Checked { get; set; }

    public required string Text { get; init; }

    public string? Hint { get; init; }

    public PropertyDefinition Definition { get; } = definition;

    public LineContentType Type { get; init; } = LineContentType.Simple;
}
