namespace Sidekick.Data.Trade.Models;

public class TradeResult<TResult>
{
    public required TResult Result { get; init; }
}