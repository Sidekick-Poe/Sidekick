using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Parser.Modifiers;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Models;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Trade;

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
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public async Task<TradeSearchResult<string>> Search(Item item, List<PropertyFilter>? propertyFilters = null, List<ModifierFilter>? modifierFilters = null, List<PseudoFilter>? pseudoFilters = null)
    {
        try
        {
            logger.LogInformation("[Trade API] Querying Trade API.");

            var query = new Query();

            var hasTypeDiscriminator = !string.IsNullOrEmpty(item.ApiInformation.Discriminator);
            if (hasTypeDiscriminator)
            {
                query.Type = new TypeDiscriminator()
                {
                    Option = item.ApiInformation.Type,
                    Discriminator = item.ApiInformation.Discriminator,
                };
            }
            else
            {
                query.Type = item.ApiInformation.Type;
            }

            if (item.ApiInformation.Category == Category.ItemisedMonster && !string.IsNullOrEmpty(item.ApiInformation.Name))
            {
                query.Term = item.ApiInformation.Name;
                query.Type = null;
            }
            else if (item.Properties.Rarity == Rarity.Unique && !string.IsNullOrEmpty(item.ApiInformation.Name))
            {
                query.Name = item.ApiInformation.Name;
            }

            var currency = item.Game == GameType.PathOfExile1 ? await settingsService.GetString(SettingKeys.PriceCheckCurrency) : await settingsService.GetString(SettingKeys.PriceCheckCurrencyPoE2);
            currency = filterProvider.GetPriceOption(currency);
            if (!string.IsNullOrEmpty(currency))
            {
                query.Filters.GetOrCreateTradeFilters().Filters.Price = new(currency);
            }

            // Stats
            var andGroup = GetAndStats(modifierFilters, pseudoFilters);
            if (andGroup != null) query.Stats.Add(andGroup);

            query.Stats.AddRange(GetCountStats(modifierFilters));
            query.Stats.AddRange(GetWeightedSumStats(pseudoFilters));

            // Properties
            if (propertyFilters != null)
            {
                propertyParser.PrepareTradeRequest(query, item, propertyFilters);
            }

            // Trade Settings
            var status = await settingsService.GetString(SettingKeys.PriceCheckStatus);
            query.Status.Option = status ?? Status.Securable;

            var league = await settingsService.GetLeague();
            var uri = new Uri($"{await GetBaseApiUrl(item.Game)}search/{league}");

            var request = new QueryRequest()
            {
                Query = query,
            };
            var json = JsonSerializer.Serialize(request, JsonSerializerOptions);

            using var body = new StringContent(json, Encoding.UTF8, "application/json");
            using var httpClient = httpClientFactory.CreateClient(TradeApiClient.ClientName);
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

    private static StatFilterGroup? GetAndStats(IEnumerable<ModifierFilter>? modifierFilters, IEnumerable<PseudoFilter>? pseudoFilters)
    {
        var andGroup = new StatFilterGroup()
        {
            Type = StatType.And,
        };

        if (modifierFilters != null)
        {
            foreach (var filter in modifierFilters)
            {
                if (filter.Checked != true || filter.Line.ApiInformation.Count != 1)
                {
                    continue;
                }

                andGroup.Filters.Add(new StatFilters()
                {
                    Id = filter.Line.ApiInformation.First().ApiId,
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

    private static IEnumerable<StatFilterGroup> GetCountStats(IEnumerable<ModifierFilter>? modifierFilters)
    {
        if (modifierFilters == null)
        {
            yield break;
        }

        var countGroup = new StatFilterGroup()
        {
            Type = StatType.Count,
            Value = new StatFilterValue()
            {
                Min = 0,
            },
        };

        foreach (var filter in modifierFilters)
        {
            if (filter.Checked != true || filter.Line.ApiInformation.Count <= 1)
            {
                continue;
            }

            var modifiers = filter.Line.ApiInformation.ToList();
            if (filter.UsePrimaryCategory)
            {
                modifiers = modifiers.Where(x => x.Category == filter.PrimaryCategory).ToList();
            }
            else if (filter.SecondaryCategory != ModifierCategory.Undefined)
            {
                modifiers = modifiers.Where(x => x.Category == filter.SecondaryCategory).ToList();
            }

            foreach (var modifier in modifiers)
            {
                countGroup.Filters.Add(new StatFilters()
                {
                    Id = modifier.ApiId,
                    Value = new StatFilterValue(filter),
                });
            }

            if (countGroup.Value != null && modifiers.Count != 0)
            {
                countGroup.Value.Min += 1;
            }
        }

        if (countGroup.Filters.Count == 0)
        {
            yield break;
        }

        yield return countGroup;
    }

    private static IEnumerable<StatFilterGroup> GetWeightedSumStats(IEnumerable<PseudoFilter>? pseudoFilters)
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

            using var httpClient = httpClientFactory.CreateClient(TradeApiClient.ClientName);
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
        var league = await settingsService.GetLeague();
        return new Uri(baseUri, $"{league}/{queryId}");
    }

    private Task<string> GetBaseApiUrl(GameType game)
    {
        return Task.FromResult(gameLanguageProvider.Language.GetTradeApiBaseUrl(game));
    }

    private Task<string> GetBaseUrl(GameType game)
    {
        return Task.FromResult(gameLanguageProvider.Language.GetTradeBaseUrl(game));
    }
}
