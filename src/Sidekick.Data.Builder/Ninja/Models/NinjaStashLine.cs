namespace Sidekick.Data.Builder.Ninja.Models;

public class NinjaStashLine
{
    public string? DetailsId { get; init; }
    public string? Name { get; init; }
    public string? BaseType { get; init; }
    public bool? Corrupted { get; init; }
    public int? GemLevel { get; init; }
    public int? GemQuality { get; init; }
    public int? Links { get; init; }
    public int? LevelRequired { get; init; }
    public string? Variant { get; init; }
    public List<NinjaStashTradeInfo>? TradeInfo { get; init; }
    public List<NinjaStashModifier>? MutatedModifiers { get; init; }
}