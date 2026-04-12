namespace Sidekick.Data.Builder.Trade.Models;

public class RawTradeResult<TResult>
{
    public required TResult Result { get; init; }
}