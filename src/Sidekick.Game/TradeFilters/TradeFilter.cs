namespace Sidekick.Game.TradeFilters;

public class TradeFilter
{
    public string? Id { get; set; }
    public string? Text { get; set; }

    public TradeFilterOptions Option { get; set; } = new();
}
