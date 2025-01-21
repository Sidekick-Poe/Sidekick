using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Models;
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
    IPoeTradeClient poeTradeClient,
    IModifierProvider modifierProvider,
    IFilterProvider filterProvider,
    IPropertyParser propertyParser
) : ITradeSearchService
{
    private readonly ILogger logger = logger;

    public async Task<TradeSearchResult<string>> Search(Item item, PropertyFilters? propertyFilters = null, List<ModifierFilter>? modifierFilters = null, List<PseudoModifierFilter>? pseudoFilters = null)
    {
        try
        {
            logger.LogInformation("[Trade API] Querying Trade API.");

            var query = new Query();
            var metadata = await GetHeader(item);

            // If the English trade is used, we must use the invariant name.
            var useInvariantTradeResults = await settingsService.GetBool(SettingKeys.UseInvariantTradeResults);
            string? itemApiNameToUse = useInvariantTradeResults ? item.Invariant?.ApiName : item.Header.ApiName;

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
                query.Filters.TypeFilters.Filters.Category = GetCategoryFilter(item.Header.ItemCategory);
            }

            if (item.Header.Category == Category.ItemisedMonster && !string.IsNullOrEmpty(itemApiNameToUse))
            {
                query.Term = itemApiNameToUse;
                query.Type = null;
            }
            else if (item.Header.Rarity == Rarity.Unique && !string.IsNullOrEmpty(itemApiNameToUse))
            {
                query.Name = itemApiNameToUse;
                query.Filters.TypeFilters.Filters.Rarity = new SearchFilterOption("Unique");
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

                query.Filters.TypeFilters.Filters.Rarity = new SearchFilterOption(rarity);
            }
            else
            {
                query.Filters.TypeFilters.Filters.Rarity = new SearchFilterOption("nonunique");
            }

            var currency = item.Header.Game == GameType.PathOfExile ? await settingsService.GetString(SettingKeys.PriceCheckItemCurrency) : await settingsService.GetString(SettingKeys.PriceCheckItemCurrencyPoE2);
            currency = filterProvider.GetPriceOption(currency);
            if (!string.IsNullOrEmpty(currency))
            {
                query.Filters.TradeFilters = new TradeFilterGroup
                {
                    Filters =
                    {
                        Price = new StatFilterValue(currency),
                    },
                };
            }

            SetSocketFilters(item, query.Filters);

            var andGroup = GetAndStats(modifierFilters, pseudoFilters);
            if (andGroup != null) query.Stats.Add(andGroup);

            var countGroup = GetCountStats(modifierFilters);
            if (countGroup != null) query.Stats.Add(countGroup);

            query.Stats.AddRange(GetWeightedSumStats(pseudoFilters));

            if (propertyFilters != null)
            {
                propertyParser.PrepareTradeRequest(query.Filters, item, propertyFilters);
            }

            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var uri = new Uri($"{await GetBaseApiUrl(metadata.Game)}search/{leagueId.GetUrlSlugForLeague()}");

            var json = JsonSerializer.Serialize(new QueryRequest() { Query = query, }, poeTradeClient.Options);

            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await poeTradeClient.HttpClient.PostAsync(uri, body);

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TradeSearchResult<string>?>(content, poeTradeClient.Options);
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

    private SearchFilterOption? GetCategoryFilter(string? itemCategory)
    {
        if (string.IsNullOrEmpty(itemCategory))
        {
            return null;
        }

        return new SearchFilterOption(itemCategory);
    }

    private StatFilterGroup? GetAndStats(List<ModifierFilter>? modifierFilters, List<PseudoModifierFilter>? pseudoFilters)
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
                    Id = filter.Line.Modifiers.First().Id,
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

    private StatFilterGroup? GetCountStats(List<ModifierFilter>? modifierFilters)
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
                        Id = modifier.Id,
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

    private List<StatFilterGroup> GetWeightedSumStats(List<PseudoModifierFilter>? pseudoFilters)
    {
        if (pseudoFilters == null)
        {
            return [];
        }

        var stats = new List<StatFilterGroup>();

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
                stats.Add(group);
            }
        }

        return stats;
    }

    private static void SetSocketFilters(Item item, SearchFilters filters)
    {
        // Auto Search 5+ Links
        var highestCount = item.Sockets.GroupBy(x => x.Group).Select(x => x.Count()).OrderByDescending(x => x).FirstOrDefault();
        if (highestCount < 5)
        {
            return;
        }

        filters.SocketFilters = new()
        {
            Filters =
            {
                Links = new SocketFilterOption()
                {
                    Min = highestCount,
                },
            },
        };
    }

    public async Task<List<TradeItem>> GetResults(GameType game, string queryId, List<string> ids, List<PseudoModifierFilter>? pseudoFilters = null)
    {
        try
        {
            logger.LogInformation($"[Trade API] Fetching Trade API Listings from Query {queryId}.");

            var response = await poeTradeClient.HttpClient.GetAsync(await GetBaseApiUrl(game) + "fetch/" + string.Join(",", ids) + "?query=" + queryId);
            if (!response.IsSuccessStatusCode)
            {
                return new();
            }

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<FetchResult<Result?>>(content,
                                                                                     new JsonSerializerOptions()
                                                                                     {
                                                                                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                                                     });
            if (result == null)
            {
                return new();
            }

            return result.Result.Where(x => x != null).ToList().ConvertAll(x => GetItem(game, x!));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"[Trade API] Exception thrown when fetching trade API listings from Query {queryId}.");
        }

        return new();
    }

    private TradeItem GetItem(GameType game, Result result)
    {
        var header = new ItemHeader()
        {
            Name = result.Item?.Name,
            Type = result.Item?.TypeLine,
            ApiItemId = "",
            ApiName = result.Item?.Name,
            ApiType = result.Item?.TypeLine,
            Category = Category.Unknown,
            Game = game,
            Rarity = result.Item?.Rarity ?? Rarity.Unknown,
        };

        var properties = new ItemProperties()
        {
            Quality = 20,
            ItemLevel = result.Item?.ItemLevel ?? 0,
            Corrupted = result.Item?.Corrupted ?? false,
            Unidentified = result.Item?.Identified ?? false,
            Armour = result.Item?.Extended?.ArmourAtMax ?? 0,
            EnergyShield = result.Item?.Extended?.EnergyShieldAtMax ?? 0,
            EvasionRating = result.Item?.Extended?.EvasionAtMax ?? 0,
            TotalDps = result.Item?.Extended?.DamagePerSecond ?? 0,
            ElementalDps = result.Item?.Extended?.ElementalDps ?? 0,
            PhysicalDps = result.Item?.Extended?.PhysicalDps ?? 0,
            BaseDefencePercentile = result.Item?.Extended?.BaseDefencePercentile ?? 0,
            Influences = result.Item?.Influences ?? new(),
        };

        var item = new TradeItem(itemHeader: header,
                                 itemProperties: properties,
                                 sockets: ParseSockets(result.Item?.Sockets).ToList(),
                                 modifierLines: new(),
                                 pseudoModifiers: new(),
                                 text: Encoding.UTF8.GetString(Convert.FromBase64String(result.Item?.Extended?.Text ?? string.Empty)))
        {
            Id = result.Id,
            Price = new TradePrice()
            {
                AccountCharacter = result.Listing?.Account?.LastCharacterName,
                AccountName = result.Listing?.Account?.Name,
                Amount = result.Listing?.Price?.Amount ?? -1,
                Currency = result.Listing?.Price?.Currency ?? "",
                Date = result.Listing?.Indexed ?? DateTimeOffset.MinValue,
                Whisper = result.Listing?.Whisper,
                Note = result.Item?.Note,
            },
            Image = result.Item?.Icon,
            Width = result.Item?.Width ?? 0,
            Height = result.Item?.Height ?? 0,
            RequirementContents = ParseLineContents(result.Item?.Requirements),
            PropertyContents = ParseLineContents(result.Item?.Properties),
            AdditionalPropertyContents = ParseLineContents(result.Item?.AdditionalProperties, false),
        };

        item.ModifierLines.AddRange(ParseModifierLines(result.Item?.EnchantMods, result.Item?.Extended?.Mods?.Enchant, ParseHash(result.Item?.Extended?.Hashes?.Enchant)));

        item.ModifierLines.AddRange(ParseModifierLines(result.Item?.RuneMods, result.Item?.Extended?.Mods?.Rune, ParseHash(result.Item?.Extended?.Hashes?.Rune)));

        item.ModifierLines.AddRange(ParseModifierLines(result.Item?.ImplicitMods ?? result.Item?.LogbookMods.SelectMany(x => x.Mods).ToList(), result.Item?.Extended?.Mods?.Implicit, ParseHash(result.Item?.Extended?.Hashes?.Implicit)));

        item.ModifierLines.AddRange(ParseModifierLines(result.Item?.CraftedMods, result.Item?.Extended?.Mods?.Crafted, ParseHash(result.Item?.Extended?.Hashes?.Crafted)));

        item.ModifierLines.AddRange(ParseModifierLines(result.Item?.ExplicitMods, result.Item?.Extended?.Mods?.Explicit, ParseHash(result.Item?.Extended?.Hashes?.Explicit, result.Item?.Extended?.Hashes?.Monster)));

        item.ModifierLines.AddRange(ParseModifierLines(result.Item?.FracturedMods, result.Item?.Extended?.Mods?.Fractured, ParseHash(result.Item?.Extended?.Hashes?.Fractured)));

        item.ModifierLines.AddRange(ParseModifierLines(result.Item?.ScourgeMods, result.Item?.Extended?.Mods?.Scourge, ParseHash(result.Item?.Extended?.Hashes?.Scourge)));

        item.ModifierLines = item.ModifierLines.OrderBy(x => item.Text.IndexOf(x.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();

        return item;
    }

    private static List<LineContentValue> ParseHash(params List<List<JsonElement>>?[] hashes)
    {
        var result = new List<LineContentValue>();

        foreach (var values in hashes)
        {
            if (values == null)
            {
                continue;
            }

            foreach (var value in values)
            {
                if (value.Count != 2)
                {
                    continue;
                }

                result.Add(new LineContentValue()
                {
                    Value = value[0].GetString(),
                    Type = value[1].ValueKind == JsonValueKind.Array ? (LineContentType)value[1][0].GetInt32() : LineContentType.Simple
                });
            }
        }

        return result;
    }

    private static List<LineContent> ParseLineContents(List<ResultLineContent>? lines, bool executeOrderBy = true)
    {
        if (lines == null)
        {
            return new();
        }

        return lines.OrderBy(x => executeOrderBy ? x.Order : 0)
            .Select(line =>
            {
                var values = new List<LineContentValue>();
                foreach (var value in line.Values)
                {
                    if (value.Count != 2)
                    {
                        continue;
                    }

                    values.Add(new LineContentValue()
                    {
                        Value = value[0].GetString(),
                        Type = (LineContentType)value[1].GetInt32()
                    });
                }

                var text = line.Name;

                if (values.Count <= 0)
                {
                    if (text != null) text = ModifierProvider.RemoveSquareBrackets(text);
                    return new LineContent()
                    {
                        Text = text,
                        Values = values,
                    };
                }

                switch (line.DisplayMode)
                {
                    case 0:
                        text = line.Name;
                        if (values.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(line.Name))
                            {
                                text += ": ";
                            }

                            text += string.Join(", ", values.Select(x => x.Value));
                        }

                        break;

                    case 1: text = $"{values[0].Value} {line.Name}"; break;

                    case 2: text = $"{values[0].Value}"; break;

                    case 3:
                        var format = Regex.Replace(line.Name ?? string.Empty, "%(\\d)", "{$1}");
                        text = string.Format(format, values.Select(x => x.Value).ToArray());
                        break;

                    default: text = $"{line.Name} {string.Join(", ", values.Select(x => x.Value))}"; break;
                }

                if (text != null) text = ModifierProvider.RemoveSquareBrackets(text);

                return new LineContent()
                {
                    Text = text,
                    Values = values,
                };
            })
            .ToList();
    }

    private IEnumerable<ModifierLine> ParseModifierLines(List<string>? texts, List<Mod>? mods, List<LineContentValue>? hashes)
    {
        if (texts == null || mods == null || hashes == null)
        {
            yield break;
        }

        for (var index = 0; index < hashes.Count; index++)
        {
            var id = hashes[index].Value;
            if (id == null || index >= texts.Count)
            {
                continue;
            }

            var text = texts.FirstOrDefault(x => modifierProvider.IsMatch(id, x)) ?? texts[index];
            text = ModifierProvider.RemoveSquareBrackets(text);

            var mod = mods.FirstOrDefault(x => x.Magnitudes?.Any(y => y.Hash == id) == true);

            yield return new ModifierLine(text: text)
            {
                Modifiers =
                [
                    new Modifier(text: text)
                    {
                        Id = id,
                        Category = modifierProvider.GetModifierCategory(id),
                        Tier = mod?.Tier,
                        TierName = mod?.Name,
                    },
                ],
            };
        }
    }

    private static IEnumerable<Socket> ParseSockets(List<ResultSocket>? sockets)
    {
        if (sockets == null)
        {
            return
            [
            ];
        }

        return sockets.Where(x => x.ColourString != "DV") // Remove delve resonator sockets
            .Select(x => new Socket()
            {
                Group = x.Group,
                Colour = x.ColourString switch
                {
                    "B" => SocketColour.Blue,
                    "G" => SocketColour.Green,
                    "R" => SocketColour.Red,
                    "W" => SocketColour.White,
                    "A" => SocketColour.Abyss,
                    "S" => SocketColour.Soulcore,
                    _ => x.Type switch
                    {
                        "rune" => x.Item == "rune" ? SocketColour.Rune : SocketColour.PathOfExile2,
                        _ => throw new Exception("Invalid socket"),
                    }
                }
            });
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
