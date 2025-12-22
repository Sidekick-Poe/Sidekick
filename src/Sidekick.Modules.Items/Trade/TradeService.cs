using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Apis.Poe.Trade.Trade.Bulk;
using Sidekick.Apis.Poe.Trade.Trade.Bulk.Models;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Exceptions;
namespace Sidekick.Modules.Items.Trade;

public class TradeService
(
    IBulkTradeService bulkTradeService,
    IItemTradeService itemTradeService
)
{
    public event Action? Changed;

    public TradeMode CurrentMode { get; private set; }

    public bool IsLoading { get; private set; }

    public string? ResultError { get; private set; }

    public TradeSearchResult<string>? ItemTradeResult { get; private set; }

    public List<Apis.Poe.Trade.Trade.Items.Results.TradeResult> TradeItems { get; private set; } = [];

    public BulkResponseModel? BulkTradeResult { get; private set; }

    public void Init()
    {
        IsLoading = false;
        CurrentMode = TradeMode.Item;
        Clear();
    }

    public void Clear()
    {
        ResultError = null;
        ItemTradeResult = null;
        TradeItems = [];
        BulkTradeResult = null;
        Changed?.Invoke();
    }

    public async Task SearchItems(Item item, List<TradeFilter> filters)
    {
        CurrentMode = TradeMode.Item;
        IsLoading = true;
        Clear();

        ItemTradeResult = await itemTradeService.Search(item, filters);

        IsLoading = false;
        await LoadMoreItems(item.Game);
        Changed?.Invoke();
    }

    public async Task SearchBulk(Item item)
    {
        CurrentMode = TradeMode.Bulk;
        IsLoading = true;
        Clear();

        BulkTradeResult = await bulkTradeService.SearchBulk(item);

        IsLoading = false;
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
