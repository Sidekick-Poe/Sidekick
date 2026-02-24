namespace Sidekick.Data.Trade.Models.Raw;

public class RawTradeFilterOption
{
    public string? Id { get; set; }
    public string? Text { get; set; }

    public override string ToString() => Text ?? Id ?? "";
}
