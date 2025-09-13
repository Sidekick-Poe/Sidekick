using Sidekick.Apis.Poe.Models;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.Bulk;
using Sidekick.Apis.Poe.Trade.Bulk.Models;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Exceptions;

namespace Sidekick.Modules.Trade.Trade;

public class TradeService
(
    IBulkTradeService bulkTradeService,
    ITradeSearchService tradeSearchService
)
{
    public event Action? Changed;

    public TradeMode CurrentMode { get; private set; }

    public bool IsLoading { get; private set; }

    public string? ResultError { get; private set; }

    public TradeSearchResult<string>? ItemTradeResult { get; private set; }

    public List<Apis.Poe.Trade.Trade.Results.TradeResult> TradeItems { get; private set; } = [];

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

    public async Task SearchItems(Item item, PropertyFilters propertyFilters, List<ModifierFilter> modifierFilters, List<PseudoModifierFilter> pseudoFilters)
    {
        CurrentMode = TradeMode.Item;
        IsLoading = true;
        Clear();

        ItemTradeResult = await tradeSearchService.Search(item, propertyFilters, modifierFilters, pseudoFilters);

        IsLoading = false;
        await LoadMoreItems(item.Header.Game);
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

        var ids = ItemTradeResult.Result.Skip(TradeItems?.Count ?? 0).Take(10).ToList();
        if (ids.Count == 0)
        {
            return;
        }

        IsLoading = true;
        ResultError = null;
        Changed?.Invoke();

        try
        {
            var result = await tradeSearchService.GetResults(game, ItemTradeResult.Id, ids);
            TradeItems?.AddRange(result);
        }
        catch (SidekickException e)
        {
            ResultError = e.Message;
        }

        IsLoading = false;
        Changed?.Invoke();
    }
}
