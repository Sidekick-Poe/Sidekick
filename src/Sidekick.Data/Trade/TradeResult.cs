namespace Sidekick.Data.Trade;

public class TradeResult<TResult>
{
    public required TResult Result { get; init; }
}