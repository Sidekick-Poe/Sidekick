using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Models;

namespace Sidekick.Apis.Poe.Trade.Parser.Headers.Models;

public class ItemClassDefinition
{
    public string? Id { get; init; }

    public ItemClass ItemClass { get; set; } = ItemClass.Unknown;

    public string? Text { get; init; }

    public Regex? Pattern { get; set; }

    public string? FuzzyText { get; init; }

    public override string ToString() => Text ?? string.Empty;
}
