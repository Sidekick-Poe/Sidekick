namespace Sidekick.Data.Trade.Raw;

public class RawTradeFilterOption
{
    public string? Id { get; set; }
    public string? Text { get; set; }

    public override string ToString() => Text ?? Id ?? "";
}
