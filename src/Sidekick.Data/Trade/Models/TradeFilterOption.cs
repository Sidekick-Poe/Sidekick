namespace Sidekick.Data.Trade.Models;

public class TradeFilterOption
{
    public string? Id { get; set; }
    public string? Text { get; set; }

    public override string ToString() => Text ?? Id ?? "";
}
