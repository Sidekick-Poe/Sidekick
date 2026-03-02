namespace Sidekick.Data.Trade.Raw;

public class RawTradeResult<TResult>
{
    public required TResult Result { get; init; }
}