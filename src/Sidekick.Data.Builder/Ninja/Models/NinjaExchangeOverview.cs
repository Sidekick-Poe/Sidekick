namespace Sidekick.Data.Builder.Ninja.Models;

public class NinjaExchangeOverview
{
    public NinjaOverviewCore? Core { get; init; }
    public List<NinjaExchangeItem> Items { get; init; } = [];
}
