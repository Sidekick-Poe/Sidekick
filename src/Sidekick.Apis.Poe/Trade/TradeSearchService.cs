using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
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
    IModifierProvider modifierProvider,
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

    public async Task<List<TradeItem>> GetResults(GameType game, string queryId, List<string> ids)
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
            var result = await JsonSerializer.DeserializeAsync<FetchResult<Result?>>(content, JsonSerializerOptions);
            if (result == null)
            {
                return [];
            }

            return result.Result.Where(x => x != null).ToList().ConvertAll(x => GetItem(game, x!));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[Trade API] Exception thrown when fetching trade API listings from Query {queryId}.");
            throw new SidekickException("Sidekick could not fetch the listings from the trade API.");
        }
    }

    private TradeItem GetItem(GameType game, Result result)
    {
        var header = new ItemHeader()
        {
            Name = result.Item.Name,
            Type = result.Item.TypeLine,
            ApiItemId = "",
            ApiName = result.Item.Name,
            ApiType = result.Item.TypeLine,
            Category = Category.Unknown,
            Game = game,
            Rarity = result.Item.Rarity,
        };

        var properties = new ItemProperties()
        {
            Quality = 20,
            ItemLevel = result.Item.ItemLevel,
            Corrupted = result.Item.Corrupted,
            Unidentified = result.Item.Identified is false,
            Armour = result.Item.Extended?.ArmourAtMax ?? 0,
            EnergyShield = result.Item.Extended?.EnergyShieldAtMax ?? 0,
            EvasionRating = result.Item.Extended?.EvasionAtMax ?? 0,
            TotalDps = result.Item.Extended?.DamagePerSecond ?? 0,
            ElementalDps = result.Item.Extended?.ElementalDps ?? 0,
            PhysicalDps = result.Item.Extended?.PhysicalDps ?? 0,
            BaseDefencePercentile = result.Item.Extended?.BaseDefencePercentile ?? 0,
            Influences = result.Item.Influences,
            Sockets = [.. ParseSockets(result.Item.Sockets, result.Item.GemSockets)],
        };

        return new TradeItem()
        {
            Invariant = null,
            Header = header,
            Properties = properties,
            ModifierLines = [.. GetModifierLines(result.Item)],
            PseudoModifiers = [],
            Text = Encoding.UTF8.GetString(Convert.FromBase64String(result.Item.Extended?.Text ?? string.Empty)),
            Id = result.Id,
            Price = new TradePrice()
            {
                AccountCharacter = result.Listing.Account?.LastCharacterName,
                AccountName = result.Listing.Account?.Name,
                Amount = result.Listing.Price?.Amount ?? -1,
                Currency = result.Listing.Price?.Currency ?? "",
                Date = result.Listing.Indexed,
                Whisper = result.Listing.Whisper,
                Note = result.Item.Note,
            },
            Image = result.Item.Icon,
            Width = result.Item.Width,
            Height = result.Item.Height,
            RequirementContents = ParseLineContents(result.Item.Requirements),
            PropertyContents = ParseLineContents(result.Item.Properties),
            AdditionalPropertyContents = ParseLineContents(result.Item.AdditionalProperties, false),
        };
    }

    private IEnumerable<ModifierLine> GetModifierLines(ResultItem? resultItem)
    {
        if (resultItem is null)
        {
            return [];
        }

        return GetAllModifierLines(resultItem).SelectMany(s => s).OrderBy(x => x.Text.IndexOf(x.Text, StringComparison.InvariantCultureIgnoreCase));
    }

    private IEnumerable<IEnumerable<ModifierLine>> GetAllModifierLines(ResultItem resultItem)
    {
        var index = 0;
        foreach (var logbook in resultItem.LogbookMods)
        {
            var blockIndex = ++index;
            yield return
            [
                new ModifierLine(text: logbook.Name)
                {
                    BlockIndex = blockIndex,
                    Modifiers =
                    [
                        new Modifier(text: logbook.Name)
                        {
                            Category = ModifierCategory.WhiteText,
                        },
                    ],
                },
                new ModifierLine(text: logbook.Faction.Name)
                {
                    BlockIndex = blockIndex,
                    Modifiers =
                    [
                        new Modifier(text: logbook.Faction.Name)
                        {
                            Category = logbook.Faction.Category,
                        },
                    ],
                }
            ];
            yield return ParseModifierLines(blockIndex, logbook.Mods, resultItem.Extended?.Mods?.Implicit, ParseHash(resultItem.Extended?.Hashes?.Implicit));
        }

        yield return ParseModifierLines(++index, resultItem.EnchantMods, resultItem.Extended?.Mods?.Enchant, ParseHash(resultItem.Extended?.Hashes?.Enchant));
        yield return ParseModifierLines(++index, resultItem.RuneMods, resultItem.Extended?.Mods?.Rune, ParseHash(resultItem.Extended?.Hashes?.Rune));
        yield return ParseModifierLines(++index, resultItem.ImplicitMods, resultItem.Extended?.Mods?.Implicit, ParseHash(resultItem.Extended?.Hashes?.Implicit));
        yield return ParseModifierLines(++index, resultItem.CraftedMods, resultItem.Extended?.Mods?.Crafted, ParseHash(resultItem.Extended?.Hashes?.Crafted));
        yield return ParseModifierLines(++index, resultItem.ExplicitMods, resultItem.Extended?.Mods?.Explicit, ParseHash(resultItem.Extended?.Hashes?.Explicit, resultItem.Extended?.Hashes?.Monster));
        yield return ParseModifierLines(++index, resultItem.FracturedMods, resultItem.Extended?.Mods?.Fractured, ParseHash(resultItem.Extended?.Hashes?.Fractured));
        yield return ParseModifierLines(++index, resultItem.ScourgeMods, resultItem.Extended?.Mods?.Scourge, ParseHash(resultItem.Extended?.Hashes?.Scourge));
        yield return ParseModifierLines(++index, resultItem.ExplicitMods, resultItem.Extended?.Mods?.Sanctum, ParseHash(resultItem.Extended?.Hashes?.Sanctum));
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
                        text = string.Format(format, values.Select(x => (object?)x.Value).ToArray());
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

    private IEnumerable<ModifierLine> ParseModifierLines(int block, List<string>? texts, List<Mod>? mods, List<LineContentValue>? hashes)
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
                BlockIndex = block,
                Modifiers =
                [
                    new Modifier(text: text)
                    {
                        ApiId = id,
                        Category = modifierProvider.GetModifierCategory(id),
                        Tier = mod?.Tier,
                        TierName = mod?.Name,
                    },
                ],
            };
        }
    }

    private static IEnumerable<Socket> ParseSockets(List<ResultSocket>? sockets, List<string>? gemSockets)
    {
        if (sockets == null)
        {
            return
            [
            ];
        }

        if (gemSockets is not null && gemSockets.Count > 0)
        {
            return sockets.Select(x => new Socket()
            {
                Group = x.Group,
                Colour = SocketColour.PoE2_Gem,
            });
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
                    _ => x.Type switch
                    {
                        "rune" => x.Item switch
                        {
                            "rune" => SocketColour.PoE2_Rune,
                            "soulcore" => SocketColour.PoE2_Soulcore,
                            _ => SocketColour.PoE2,
                        },
                        _ => SocketColour.Undefined,
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
