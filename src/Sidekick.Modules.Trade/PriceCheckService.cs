using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe.Bulk;
using Sidekick.Apis.Poe.Bulk.Models;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Trade;

public class PriceCheckService(
    ILogger<PriceCheckService> logger,
    IItemParser itemParser,
    ITradeFilterService tradeFilterService,
    ITradeSearchService tradeSearchService,
    IBulkTradeService bulkTradeService,
    ISettingsService settingsService)
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

    public void UpdateTypeFilter(bool enabled)
    {
        if (PropertyFilters != null)
        {
            PropertyFilters.TypeFilterEnabled = enabled;
        }
    }

    public void UpdateUseSpecificType(bool enabled)
    {
        if (PropertyFilters != null)
        {
            PropertyFilters.UseSpecificType = enabled;
        }
    }

    public void UpdateRarityFilter(bool enabled)
    {
        if (PropertyFilters != null)
        {
            PropertyFilters.RarityFilterEnabled = enabled;
        }
    }

    public async Task Initialize(string itemText)
    {
        logger.LogInformation("[PriceCheck] Starting initialization");
        IsFilterLoading = true;
        FilterLoadingChanged?.Invoke();

        Item = await itemParser.ParseItemAsync(itemText.DecodeBase64Url() ?? string.Empty);
        logger.LogInformation($"[PriceCheck] Parsed item: {Item?.Header.Name} {Item?.Header.Type}");
        if (Item?.Properties.PhysicalDamage != null)
        {
            logger.LogInformation($"[PriceCheck] Physical Damage: {Item.Properties.PhysicalDamage.Min}-{Item.Properties.PhysicalDamage.Max}");
        }
        if (Item?.Properties.ElementalDamages != null)
        {
            foreach (var damage in Item.Properties.ElementalDamages)
            {
                logger.LogInformation($"[PriceCheck] Elemental Damage: {damage.Min}-{damage.Max}");
            }
        }

        if (Item != null)
        {
            PropertyFilters = tradeFilterService.GetPropertyFilters(Item);
            PropertyFilters.TypeFilterEnabled = true;
            ModifierFilters = tradeFilterService
                              .GetModifierFilters(Item)
                              .ToList();
            PseudoFilters = tradeFilterService
                            .GetPseudoModifierFilters(Item)
                            .ToList();
        }
        else
        {
            PropertyFilters = null;
            ModifierFilters = [];
            PseudoFilters = [];
        }

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

        await Search();
    }

    public async Task Search()
    {
        if (Item == null)
        {
            return;
        }

        logger.LogInformation("[PriceCheck] Starting search");
        ItemTradeResult = null;
        TradeItems = new();
        IsLoading = true;
        LoadingChanged?.Invoke();

        var currency = await settingsService.GetEnum<TradeCurrency>(SettingKeys.PriceCheckItemCurrency);
        ItemTradeResult = await tradeSearchService.Search(Item, currency ?? TradeCurrency.Divine, PropertyFilters, ModifierFilters, PseudoFilters);

        if (ItemTradeResult?.Result != null)
        {
            await LoadMoreItems();
        }

        IsLoading = false;
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
        if (result != null)
        {
            logger.LogInformation($"[PriceCheck] Got {result.Count} results");
            foreach (var item in result)
            {
                if (item.Properties.PhysicalDamage != null)
                {
                    logger.LogInformation($"[PriceCheck] Result Physical Damage: {item.Properties.PhysicalDamage.Min}-{item.Properties.PhysicalDamage.Max}");
                }
                if (item.Properties.ElementalDamages != null)
                {
                    foreach (var damage in item.Properties.ElementalDamages)
                    {
                        logger.LogInformation($"[PriceCheck] Result Elemental Damage: {damage.Min}-{damage.Max}");
                    }
                }
            }
            TradeItems?.AddRange(result);
        }

        IsLoading = false;
        LoadingChanged?.Invoke();
    }

    public async Task BulkSearch()
    {
        if (Item == null || !SupportsBulk)
        {
            return;
        }

        try
        {
            IsLoading = true;
            LoadingChanged?.Invoke();

            logger.LogInformation("[PriceCheck] Starting bulk search");
            var currency = await settingsService.GetEnum<TradeCurrency>(SettingKeys.PriceCheckBulkCurrency);
            var minStock = await settingsService.GetInt(SettingKeys.PriceCheckBulkMinimumStock);
            BulkTradeResult = await bulkTradeService.SearchBulk(Item, currency ?? TradeCurrency.Divine, minStock);
            CurrentMode = TradeMode.Bulk;
        }
        finally
        {
            IsLoading = false;
            LoadingChanged?.Invoke();
        }
    }

    public async Task ItemSearch()
    {
        if (Item == null)
        {
            return;
        }

        try
        {
            IsLoading = true;
            LoadingChanged?.Invoke();

            logger.LogInformation("[PriceCheck] Starting item search");
            var currency = await settingsService.GetEnum<TradeCurrency>(SettingKeys.PriceCheckItemCurrency);
            ItemTradeResult = await tradeSearchService.Search(Item, currency ?? TradeCurrency.Divine, PropertyFilters, ModifierFilters, PseudoFilters);
            
            if (ItemTradeResult?.Result?.Count > 0 && ItemTradeResult?.Id != null)
            {
                TradeItems = await tradeSearchService.GetResults(Item.Metadata.Game, ItemTradeResult.Id, ItemTradeResult.Result.Take(10).ToList(), PseudoFilters);
            }
            else
            {
                TradeItems = [];
            }

            CurrentMode = TradeMode.Item;
        }
        finally
        {
            IsLoading = false;
            LoadingChanged?.Invoke();
        }
    }
}
