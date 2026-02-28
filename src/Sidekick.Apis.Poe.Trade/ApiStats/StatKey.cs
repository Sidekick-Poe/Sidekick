namespace Sidekick.Apis.Poe.Trade.ApiStats;

public struct StatKey(string tradeId, int? optionId) : IEqualityComparer<StatKey>
{
    public string TradeId { get; set; } = tradeId;

    public int? OptionId { get; set; } = optionId;

    public bool Equals(StatKey x, StatKey y)
    {
        return x.TradeId == y.TradeId && x.OptionId == y.OptionId;
    }

    public int GetHashCode(StatKey obj)
    {
        return HashCode.Combine(obj.TradeId, obj.OptionId);
    }
}
