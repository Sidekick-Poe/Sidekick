namespace Sidekick.Data.Trade.Models.Raw;

public class RawTradeResult<TResult>
{
    public required TResult Result { get; init; }
}