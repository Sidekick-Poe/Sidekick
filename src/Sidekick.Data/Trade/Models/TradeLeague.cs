namespace Sidekick.Data.Trade.Models;

public class TradeLeague
{
    public string? Id { get; set; }

    public string? Text { get; set; }

    public TradeLeagueRealm Realm { get; set; }
}
