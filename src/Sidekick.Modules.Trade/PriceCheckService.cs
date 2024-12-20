using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe.Bulk;
using Sidekick.Apis.Poe.Bulk.Models;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Trade;

public class PriceCheckService(
    ISettingsService settingsService,
    IBulkTradeService bulkTradeService,
    IItemParser itemParser,
    ITradeFilterService tradeFilterService,
    ITradeSearchService tradeSearchService)
{
    public event Action? LoadingChanged;

    public event Action? FilterLoadingChanged;

    public Item? Item { get; private set; }

    public TradeMode CurrentMode { get; private set; }

    public PropertyFilters? PropertyFilters { get; private set; }

    public List<ModifierFilter> ModifierFilters { get; private set; } =
    [
    ];

    public List<PseudoModifierFilter> PseudoFilters { get; private set; } =
    [
    ];

    public bool IsLoading { get; private set; }

    public bool IsFilterLoading { get; private set; }

    public TradeSearchResult<string>? ItemTradeResult { get; private set; }

    public List<TradeItem>? TradeItems { get; private set; }

    public BulkResponseModel? BulkTradeResult { get; private set; }

    public bool SupportsBulk { get; private set; }

    public async Task Initialize(string itemText)
    {
        IsFilterLoading = true;
        FilterLoadingChanged?.Invoke();

        Item = await itemParser.ParseItemAsync(itemText.DecodeBase64Url() ?? string.Empty);

        PropertyFilters = tradeFilterService.GetPropertyFilters(Item);
        ModifierFilters = tradeFilterService
                          .GetModifierFilters(Item)
                          .ToList();
        PseudoFilters = tradeFilterService
                        .GetPseudoModifierFilters(Item)
                        .ToList();

        SupportsBulk = bulkTradeService.SupportsBulkTrade(Item);
        if (SupportsBulk)
        {
            CurrentMode = await settingsService.GetEnum<TradeMode>(SettingKeys.PriceCheckCurrencyMode) ?? TradeMode.Item;
        }
        else
        {
            CurrentMode = TradeMode.Item;
        }

        IsFilterLoading = false;
        FilterLoadingChanged?.Invoke();

        if (Item.Metadata.Rarity != Rarity.Rare && Item.Metadata.Rarity != Rarity.Magic)
        {
            if (CurrentMode == TradeMode.Bulk)
            {
                await BulkSearch();
            }
            else
            {
                await ItemSearch();
            }
        }
    }

    public async Task ItemSearch()
    {
        if (Item == null)
        {
            return;
        }

        CurrentMode = TradeMode.Item;
        TradeItems = new List<TradeItem>();
        IsLoading = true;
        LoadingChanged?.Invoke();

        ItemTradeResult = await tradeSearchService.Search(
            Item,
            PropertyFilters,
            ModifierFilters,
            PseudoFilters);

        IsLoading = false;
        await LoadMoreItems();

        LoadingChanged?.Invoke();
    }

    public async Task LoadMoreItems()
    {
        if (IsLoading || ItemTradeResult?.Result == null || ItemTradeResult?.Id == null || Item == null)
        {
            return;
        }

        var ids = ItemTradeResult
                  .Result.Skip(TradeItems?.Count ?? 0)
                  .Take(10)
                  .ToList();
        if (ids.Count == 0)
        {
            return;
        }

        IsLoading = true;
        LoadingChanged?.Invoke();

        var result = await tradeSearchService.GetResults(Item.Metadata.Game, ItemTradeResult.Id, ids, PseudoFilters);
        TradeItems?.AddRange(result);

        IsLoading = false;
        LoadingChanged?.Invoke();
    }

    public async Task BulkSearch()
    {
        if (Item == null)
        {
            return;
        }

        CurrentMode = TradeMode.Bulk;
        BulkTradeResult = null;
        IsLoading = true;
        LoadingChanged?.Invoke();

        BulkTradeResult = await bulkTradeService.SearchBulk(Item);

        IsLoading = false;
        LoadingChanged?.Invoke();
    }
}
