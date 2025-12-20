using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Models;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters;

public class TradeFilterProvider
(
    ITradeApiClient tradeApiClient,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    ICacheProvider cacheProvider,
    IStringLocalizer<PoeResources> resources
) : ITradeFilterProvider
{
    public ApiFilter? TypeCategory { get; private set; }

    public ApiFilter? TradePrice { get; private set; }

    public ApiFilter? TradeIndexed { get; private set; }

    public ApiFilter? Desecrated { get; private set; }

    public ApiFilter? Veiled { get; private set; }

    public ApiFilter? Fractured { get; private set; }

    public ApiFilter? Mirrored { get; private set; }

    public ApiFilter? Foulborn { get; private set; }

    public ApiFilter? Sanctified { get; private set; }

    public ApiFilterCategory? RequirementsCategory { get; private set; }

    public ApiFilterCategory? MiscellaneousCategory { get; private set; }

    public ApiFilterCategory? TradeCategory { get; private set; }

    private List<ApiFilterCategory> Filters { get; set; } = [];

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantFilters";

        var result = await cacheProvider.GetOrSet(cacheKey,
                                                  () => tradeApiClient.FetchData<ApiFilterCategory>(game, gameLanguageProvider.Language, "filters"),
                                                  (cache) =>
                                                  {
                                                      return cache.Result.Any(x => x.Id == "type_filters") && cache.Result.Any(x => x.Id == "trade_filters");
                                                  });
        if (result == null) throw new SidekickException("Could not fetch filters from the trade API.");

        Filters = result.Result;

        TypeCategory = GetApiFilter("type_filters", "category");
        TradePrice = GetApiFilter("trade_filters", "price");
        TradeIndexed = GetApiFilter("trade_filters", "indexed");

        RequirementsCategory = GetApiFilterCategory("req_filters");
        MiscellaneousCategory = GetApiFilterCategory("misc_filters");
        TradeCategory = GetApiFilterCategory("trade_filters");
        Desecrated = GetApiFilter("misc_filters", "desecrated");
        Veiled = GetApiFilter("misc_filters", "veiled");
        Fractured = GetApiFilter("misc_filters", "fractured_item");
        Mirrored = GetApiFilter("misc_filters", "mirrored");
        Foulborn = GetApiFilter("misc_filters", "foulborn_item");
        Sanctified = GetApiFilter("misc_filters", "sanctified");
    }

    public string? GetPriceOption(string? price) => TradePrice?.Option.Options.SingleOrDefault(x => x.Id == price)?.Id;

    public string? GetTradeIndexedOption(string? id) => TradeIndexed?.Option.Options.SingleOrDefault(x => x.Id == id)?.Id;

    private ApiFilterCategory? GetApiFilterCategory(string categoryId)
    {
        return Filters.FirstOrDefault(x => x.Id == categoryId);
    }

    private ApiFilter? GetApiFilter(string categoryId, string filterId)
    {
        return GetApiFilterCategory(categoryId)?.Filters.FirstOrDefault(x => x.Id == filterId);
    }

    public async Task<List<TradeFilter>> GetFilters(Item item)
    {
        if (TradeCategory?.Title == null) return [];

        var result = new List<TradeFilter>();

        var statusFilters = GetApiFilter("status_filters", "status");
        var statusValue = await settingsService.GetString(SettingKeys.PriceCheckStatus);
        if (statusFilters != null)
        {
            var filter = new OptionFilter()
            {
                Text = resources["Player_Status"],
                Value = statusValue ?? Status.Securable,
                DefaultValue = Status.Securable,
                SettingKey = SettingKeys.PriceCheckStatus,
                Options = statusFilters.Option.Options
                    .Select(x => new OptionFilter.OptionFilterValue(x.Id, x.Text))
                    .ToList(),
            };
            filter.PrepareTradeRequest = (query, _) => query.Status.Option = filter.Value ?? Status.Securable;
            result.Add(filter);
        }

        var priceFilters = GetApiFilter("trade_filters", "price");
        var priceKey = item.Game == GameType.PathOfExile1 ? SettingKeys.PriceCheckCurrency : SettingKeys.PriceCheckCurrencyPoE2;
        var priceValue = await settingsService.GetString(priceKey);
        if (priceFilters != null)
        {
            var filter = new OptionFilter()
            {
                Text = priceFilters.Text ?? string.Empty,
                Value = priceValue,
                DefaultValue = null,
                SettingKey = priceKey,
                Options = priceFilters.Option.Options
                    .Select(x => new OptionFilter.OptionFilterValue(x.Id, x.Text))
                    .ToList(),
            };
            filter.PrepareTradeRequest = (query, _) =>
            {
                var option = GetPriceOption(filter.Value);
                if (!string.IsNullOrEmpty(option))
                {
                    query.Filters.GetOrCreateTradeFilters().Filters.Price = new(option);
                }
            };
            result.Add(filter);
        }

        if (result.Count == 0) return [];
        return [new ExpandableFilter(TradeCategory.Title, result.ToArray())];
    }
}
