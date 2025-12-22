namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class OptionFilter : TradeFilter
{
    public record OptionFilterValue(string? Value, string? Text);

    public string? Value { get; set; }

    public virtual string? DefaultValue { get; init; }

    public override bool Checked => Value != DefaultValue;

    public required List<OptionFilterValue> Options { get; set; }
}
