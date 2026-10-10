namespace Sidekick.Apis.Poe.Trade.Results;

public class TradeSearchResult<T>
{
    public List<T>? Result { get; set; }

    public string? Id { get; set; }

    public int Total { get; set; }

    public Error? Error { get; set; }
}
