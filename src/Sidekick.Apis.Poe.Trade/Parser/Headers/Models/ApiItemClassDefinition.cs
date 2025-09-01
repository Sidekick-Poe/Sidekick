
namespace Sidekick.Apis.Poe.Trade.Parser.Headers.Models;

public class ApiItemClassDefinition
{
    public string? Id { get; init; }

    public string? Text { get; init; }

    public string? FuzzyText { get; init; }

    public override string ToString() => Text ?? string.Empty;
}
