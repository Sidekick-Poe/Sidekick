using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Parser.Headers.Models;

public class ItemCategory
{
    public string? Id { get; init; }
    public string? Text { get; init; }
    public Regex? Pattern { get; init; }
    public string? FuzzyText { get; init; }
}
