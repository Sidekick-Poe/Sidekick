namespace Sidekick.Data.Trade.Models.Raw;

public class RawTradeLeague
{
    public string? Id { get; set; }

    public string? Text { get; set; }

    public TradeLeagueRealm Realm { get; set; }
}
