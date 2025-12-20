namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class OptionFilter : TradeFilter
{
    public record OptionFilterValue(string? Value, string? Text);

    public string? Value { get; set; }

    public string? DefaultValue { get; set; }

    public override bool Checked => Value != DefaultValue;

    public required List<OptionFilterValue> Options { get; set; }

    public string? SettingKey { get; set; }
}
