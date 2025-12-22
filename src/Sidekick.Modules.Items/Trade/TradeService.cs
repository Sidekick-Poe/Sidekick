using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Exceptions;
namespace Sidekick.Modules.Items.Trade;

public class TradeService
(
    IItemTradeService itemTradeService
)
{
    public event Action? Changed;

    public bool IsLoading { get; private set; }

    public string? ResultError { get; private set; }

    public TradeSearchResult<string>? ItemTradeResult { get; private set; }

    public List<Apis.Poe.Trade.Trade.Items.Results.TradeResult> TradeItems { get; private set; } = [];

    public void Init()
    {
        IsLoading = false;
        Clear();
    }

    public void Clear()
    {
        ResultError = null;
        ItemTradeResult = null;
        TradeItems = [];
        Changed?.Invoke();
    }

    public async Task SearchItems(Item item, List<TradeFilter> filters)
    {
        IsLoading = true;
        Clear();

        ItemTradeResult = await itemTradeService.Search(item, filters);

        IsLoading = false;
        await LoadMoreItems(item.Game);
        Changed?.Invoke();
    }

    public async Task LoadMoreItems(GameType game)
    {
        if (IsLoading || ItemTradeResult?.Result == null || ItemTradeResult?.Id == null)
        {
            return;
        }

        var ids = ItemTradeResult.Result.Skip(TradeItems.Count).Take(10).ToList();
        if (ids.Count == 0)
        {
            return;
        }

        IsLoading = true;
        ResultError = null;
        Changed?.Invoke();

        try
        {
            var result = await itemTradeService.GetResults(game, ItemTradeResult.Id, ids);
            TradeItems.AddRange(result);
        }
        catch (SidekickException e)
        {
            ResultError = e.Message;
        }

        IsLoading = false;
        Changed?.Invoke();
    }
}
