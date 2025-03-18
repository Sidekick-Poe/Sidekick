using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Models;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade;

public class TradeSearchService
(
    ILogger<TradeSearchService> logger,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    IFilterProvider filterProvider,
    IPropertyParser propertyParser,
    IHttpClientFactory httpClientFactory
) : ITradeSearchService
{
    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task<TradeSearchResult<string>> Search(Item item, PropertyFilters? propertyFilters = null, List<ModifierFilter>? modifierFilters = null, List<PseudoModifierFilter>? pseudoFilters = null)
    {
        try
        {
            logger.LogInformation("[Trade API] Querying Trade API.");

            var query = new Query();
            var metadata = await GetHeader(item);

            // If the English trade is used, we must use the invariant name.
            var useInvariantTradeResults = await settingsService.GetBool(SettingKeys.UseInvariantTradeResults);
            var itemApiNameToUse = useInvariantTradeResults ? item.Invariant?.ApiName : item.Header.ApiName;

            if (propertyFilters?.BaseTypeFilterApplied ?? true)
            {
                var hasTypeDiscriminator = !string.IsNullOrEmpty(metadata.ApiDiscriminator);
                if (hasTypeDiscriminator)
                {
                    query.Type = new TypeDiscriminator()
                    {
                        Option = metadata.ApiType,
                        Discriminator = metadata.ApiDiscriminator,
                    };
                }
                else
                {
                    query.Type = metadata.ApiType;
                }
            }
            else if (propertyFilters.ClassFilterApplied)
            {
                query.Filters.GetOrCreateTypeFilters().Filters.Category = GetCategoryFilter(item.Header.ItemCategory);
            }

            if (item.Header.Category == Category.ItemisedMonster && !string.IsNullOrEmpty(itemApiNameToUse))
            {
                query.Term = itemApiNameToUse;
                query.Type = null;
            }
            else if (item.Header.Rarity == Rarity.Unique && !string.IsNullOrEmpty(itemApiNameToUse))
            {
                query.Name = itemApiNameToUse;
                query.Filters.GetOrCreateTypeFilters().Filters.Rarity = new SearchFilterOption("Unique");
            }
            else if (propertyFilters?.RarityFilterApplied ?? false)
            {
                var rarity = item.Header.Rarity switch
                {
                    Rarity.Normal => "normal",
                    Rarity.Magic => "magic",
                    Rarity.Rare => "rare",
                    Rarity.Unique => "unique",
                    _ => "nonunique",
                };

                query.Filters.GetOrCreateTypeFilters().Filters.Rarity = new SearchFilterOption(rarity);
            }

            var currency = item.Header.Game == GameType.PathOfExile ? await settingsService.GetString(SettingKeys.PriceCheckCurrency) : await settingsService.GetString(SettingKeys.PriceCheckCurrencyPoE2);
            var currencyMin = item.Header.Game == GameType.PathOfExile ? await settingsService.GetInt(SettingKeys.PriceCheckItemCurrencyMin) : await settingsService.GetInt(SettingKeys.PriceCheckItemCurrencyMinPoE2);
            var currencyMax = item.Header.Game == GameType.PathOfExile ? await settingsService.GetInt(SettingKeys.PriceCheckItemCurrencyMax) : await settingsService.GetInt(SettingKeys.PriceCheckItemCurrencyMaxPoE2);
            currency = filterProvider.GetPriceOption(currency);
            if (!string.IsNullOrEmpty(currency) || currencyMin > 0 || currencyMax > 0)
            {
                query.Filters.GetOrCreateTradeFilters().Filters.Price = new(currency)
                {
                    Min = currencyMin > 0 ? currencyMin : null,
                    Max = currencyMax > 0 ? currencyMax : null,
                };
            }

            var timeFrame = await settingsService.GetString(SettingKeys.PriceCheckItemListedAge);
            if (!string.IsNullOrWhiteSpace(timeFrame))
            {
                query.Filters.GetOrCreateTradeFilters().Filters.Indexed = new(timeFrame);
            }

            // Stats
            var andGroup = GetAndStats(modifierFilters, pseudoFilters);
            if (andGroup != null) query.Stats.Add(andGroup);

            var countGroup = GetCountStats(modifierFilters);
            if (countGroup != null) query.Stats.Add(countGroup);

            query.Stats.AddRange(GetWeightedSumStats(pseudoFilters));

            // Properties
            if (propertyFilters != null)
            {
                propertyParser.PrepareTradeRequest(query.Filters, item, propertyFilters);
            }

            // Trade Settings
            var status = await settingsService.GetString(SettingKeys.PriceCheckStatus);
            query.Status.Option = status ?? Status.Online;

            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var uri = new Uri($"{await GetBaseApiUrl(metadata.Game)}search/{leagueId.GetUrlSlugForLeague()}");

            var request = new QueryRequest()
            {
                Query = query,
            };
            var json = JsonSerializer.Serialize(request, JsonSerializerOptions);

            using var body = new StringContent(json, Encoding.UTF8, "application/json");
            using var httpClient = httpClientFactory.CreateClient(ClientNames.TradeClient);
            var response = await httpClient.PostAsync(uri, body);

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TradeSearchResult<string>?>(content, JsonSerializerOptions);
            if (result != null)
            {
                return result;
            }
        }
        catch (SidekickException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[Trade API] Exception thrown while querying trade api.");
        }

        throw new ApiErrorException();
    }

    private static SearchFilterOption? GetCategoryFilter(string? itemCategory)
    {
        if (string.IsNullOrEmpty(itemCategory))
        {
            return null;
        }

        return new SearchFilterOption(itemCategory);
    }

    private static StatFilterGroup? GetAndStats(IEnumerable<ModifierFilter>? modifierFilters, IEnumerable<PseudoModifierFilter>? pseudoFilters)
    {
        var andGroup = new StatFilterGroup()
        {
            Type = StatType.And,
        };

        if (modifierFilters != null)
        {
            foreach (var filter in modifierFilters)
            {
                if (filter.Checked != true || filter.Line.Modifiers.Count != 1)
                {
                    continue;
                }

                andGroup.Filters.Add(new StatFilters()
                {
                    Id = filter.Line.Modifiers.First().ApiId,
                    Value = new StatFilterValue(filter),
                });
            }
        }

        if (pseudoFilters != null)
        {
            foreach (var pseudoFilter in pseudoFilters)
            {
                if (pseudoFilter.Checked != true || string.IsNullOrEmpty(pseudoFilter.PseudoModifier.ModifierId))
                {
                    continue;
                }

                andGroup.Filters.Add(new StatFilters()
                {
                    Id = pseudoFilter.PseudoModifier.ModifierId,
                    Value = new StatFilterValue()
                    {
                        Min = pseudoFilter.Min,
                        Max = pseudoFilter.Max,
                    },
                });
            }
        }

        if (andGroup.Filters.Count == 0)
        {
            return null;
        }

        return andGroup;
    }

    private static StatFilterGroup? GetCountStats(IEnumerable<ModifierFilter>? modifierFilters)
    {
        var countGroup = new StatFilterGroup()
        {
            Type = StatType.Count,
            Value = new StatFilterValue()
            {
                Min = 0,
            },
        };

        if (modifierFilters != null)
        {
            foreach (var filter in modifierFilters)
            {
                if (filter.Checked != true || filter.Line.Modifiers.Count <= 1)
                {
                    continue;
                }

                var modifiers = filter.Line.Modifiers;
                if (filter.ForceFirstCategory)
                {
                    modifiers = modifiers.Where(x => x.Category == filter.FirstCategory).ToList();
                }
                else if (modifiers.Any(x => x.Category == ModifierCategory.Explicit))
                {
                    modifiers = modifiers.Where(x => x.Category == ModifierCategory.Explicit).ToList();
                }

                foreach (var modifier in modifiers)
                {
                    countGroup.Filters.Add(new StatFilters()
                    {
                        Id = modifier.ApiId,
                        Value = new StatFilterValue(filter),
                    });
                }

                if (countGroup.Value != null && modifiers.Any())
                {
                    countGroup.Value.Min += 1;
                }
            }
        }

        if (countGroup.Filters.Count == 0)
        {
            return null;
        }

        return countGroup;
    }

    private static IEnumerable<StatFilterGroup> GetWeightedSumStats(IEnumerable<PseudoModifierFilter>? pseudoFilters)
    {
        if (pseudoFilters == null)
        {
            yield break;
        }

        foreach (var filter in pseudoFilters)
        {
            if (filter.Checked != true || filter.PseudoModifier.WeightedSumModifiers.Count == 0)
            {
                continue;
            }

            var group = new StatFilterGroup()
            {
                Type = StatType.WeightedSum,
                Value = new StatFilterValue()
                {
                    Min = filter.Min,
                    Max = filter.Max,
                },
            };

            foreach (var modifier in filter.PseudoModifier.WeightedSumModifiers)
            {
                group.Filters.Add(new StatFilters()
                {
                    Id = modifier.Key,
                    Value = new StatFilterValue()
                    {
                        Weight = modifier.Value,
                    },
                });
            }

            if (group.Filters.Count > 0)
            {
                yield return group;
            }
        }
    }

    public async Task<List<TradeResult>> GetResults(GameType game, string queryId, List<string> ids)
    {
        try
        {
            logger.LogInformation($"[Trade API] Fetching Trade API Listings from Query {queryId}.");

            using var httpClient = httpClientFactory.CreateClient(ClientNames.TradeClient);
            var response = await httpClient.GetAsync(await GetBaseApiUrl(game) + "fetch/" + string.Join(",", ids) + "?query=" + queryId);
            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<FetchResult<TradeResult?>>(content, JsonSerializerOptions);
            if (result == null)
            {
                return [];
            }

            return result.Result.Where(x => x != null).ToList()!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[Trade API] Exception thrown when fetching trade API listings from Query {queryId}.");
            throw new SidekickException("Sidekick could not fetch the listings from the trade API.");
        }
    }

    public async Task<Uri> GetTradeUri(GameType game, string queryId)
    {
        var baseUri = new Uri(await GetBaseUrl(game) + "search/");
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        return new Uri(baseUri, $"{leagueId.GetUrlSlugForLeague()}/{queryId}");
    }

    private async Task<string> GetBaseApiUrl(GameType game)
    {
        var useInvariant = await settingsService.GetBool(SettingKeys.UseInvariantTradeResults);
        return useInvariant ? gameLanguageProvider.InvariantLanguage.GetTradeApiBaseUrl(game) : gameLanguageProvider.Language.GetTradeApiBaseUrl(game);
    }

    private async Task<string> GetBaseUrl(GameType game)
    {
        var useInvariant = await settingsService.GetBool(SettingKeys.UseInvariantTradeResults);
        return useInvariant ? gameLanguageProvider.InvariantLanguage.GetTradeBaseUrl(game) : gameLanguageProvider.Language.GetTradeBaseUrl(game);
    }

    private async Task<ItemHeader> GetHeader(Item item)
    {
        var useInvariant = await settingsService.GetBool(SettingKeys.UseInvariantTradeResults);
        return useInvariant ? item.Invariant ?? item.Header : item.Header;
    }
}
