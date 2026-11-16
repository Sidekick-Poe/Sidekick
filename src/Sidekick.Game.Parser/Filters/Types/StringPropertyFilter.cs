namespace Sidekick.Game.Parser.Filters.Types;

public class StringPropertyFilter : TradeFilter
{
    public required string Value { get; init; }

    /// <summary>
    /// Shown but not editable, e.g. when the value is already part of the trade item being searched.
    /// </summary>
    public bool Readonly { get; init; }
}
