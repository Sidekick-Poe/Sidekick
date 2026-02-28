namespace Sidekick.Data.Builder.Repoe.Models.Poe1;

public class RepoeStatTrade
{
    public required string Id { get; set; }

    public required string Text { get; set; }

    public required string Type { get; set; }

    public RepoeStatTradeOptions? Options { get; set; }
}