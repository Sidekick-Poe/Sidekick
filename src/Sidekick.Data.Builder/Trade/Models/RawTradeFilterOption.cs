namespace Sidekick.Data.Builder.Trade.Models;

public class RawTradeFilterOption
{
    public string? Id { get; set; }
    public string? Text { get; set; }

    public override string ToString() => Text ?? Id ?? "";
}
