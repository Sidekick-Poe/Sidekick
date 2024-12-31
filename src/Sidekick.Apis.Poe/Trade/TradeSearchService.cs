using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Modifiers;
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
    IApiInvariantItemProvider apiInvariantItemProvider
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

            if (item.Header.Category == Category.ItemisedMonster)
            {
                if (!string.IsNullOrEmpty(item.Header.ApiName))
                {
                    query.Term = item.Header.ApiName;
                    query.Type = null;
                }
            }
            else if (item.Header.Rarity == Rarity.Unique)
            {
                query.Name = item.Header.ApiName;
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
                query.Filters.EquipmentFilters = GetEquipmentFilters(item, propertyFilters.Weapon.Concat(propertyFilters.Armour).ToList());
                query.Filters.WeaponFilters = GetWeaponFilters(item, propertyFilters.Weapon);
                query.Filters.ArmourFilters = GetArmourFilters(item, propertyFilters.Armour);
                query.Filters.MapFilters = GetMapFilters(propertyFilters.Map);
                query.Filters.MiscFilters = GetMiscFilters(item, propertyFilters.Misc);
            }

            // The item level filter for Path of Exile 2 is inside the type filters instead of the misc filters.
            if (item.Header.Game == GameType.PathOfExile2)
            {
                query.Filters.TypeFilters.Filters.ItemLevel = query.Filters.MiscFilters?.Filters.ItemLevel;
                if (query.Filters.MiscFilters != null)
                {
                    query.Filters.MiscFilters.Filters.ItemLevel = null;
                }
            }

            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var uri = new Uri($"{await GetBaseApiUrl(metadata.Game)}search/{leagueId.GetUrlSlugForLeague()}");

            var json = JsonSerializer.Serialize(new QueryRequest()
                                                {
                                                    Query = query,
                                                },
                                                poeTradeClient.Options);

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

    private WeaponFilterGroup? GetWeaponFilters(Item item, List<PropertyFilter> propertyFilters)
    {
        if (item.Header.Game == GameType.PathOfExile2)
        {
            return null;
        }

        var filters = new WeaponFilterGroup();
        var hasValue = false;

        foreach (var propertyFilter in propertyFilters)
        {
            if (propertyFilter.Checked != true)
            {
                continue;
            }

            switch (propertyFilter.Type)
            {
                case PropertyFilterType.Weapon_Damage:
                    filters.Filters.Damage = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_PhysicalDps:
                    filters.Filters.PhysicalDps = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_ElementalDps:
                    filters.Filters.ElementalDps = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_Dps:
                    filters.Filters.DamagePerSecond = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_AttacksPerSecond:
                    filters.Filters.AttacksPerSecond = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_CriticalStrikeChance:
                    filters.Filters.CriticalStrikeChance = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;
            }
        }

        return hasValue ? filters : null;
    }

    private ArmourFilterGroup? GetArmourFilters(Item item, List<PropertyFilter> propertyFilters)
    {
        if (item.Header.Game == GameType.PathOfExile2)
        {
            return null;
        }

        var filters = new ArmourFilterGroup();
        var hasValue = false;

        foreach (var propertyFilter in propertyFilters)
        {
            if (propertyFilter.Checked != true)
            {
                continue;
            }

            switch (propertyFilter.Type)
            {
                case PropertyFilterType.Armour_Armour:
                    filters.Filters.Armor = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Block:
                    filters.Filters.Block = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_EnergyShield:
                    filters.Filters.EnergyShield = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Evasion:
                    filters.Filters.Evasion = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;
            }
        }

        return hasValue ? filters : null;
    }

    private EquipmentFilterGroup? GetEquipmentFilters(Item item, List<PropertyFilter> propertyFilters)
    {
        if (item.Header.Game == GameType.PathOfExile)
        {
            return null;
        }

        var filters = new EquipmentFilterGroup();
        var hasValue = false;

        foreach (var propertyFilter in propertyFilters)
        {
            if (propertyFilter.Checked != true)
            {
                continue;
            }

            switch (propertyFilter.Type)
            {
                case PropertyFilterType.Weapon_Damage:
                    filters.Filters.Damage = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_PhysicalDps:
                    filters.Filters.PhysicalDps = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_ElementalDps:
                    filters.Filters.ElementalDps = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_Dps:
                    filters.Filters.DamagePerSecond = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_AttacksPerSecond:
                    filters.Filters.AttacksPerSecond = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_CriticalStrikeChance:
                    filters.Filters.CriticalStrikeChance = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Armour:
                    filters.Filters.Armor = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Block:
                    filters.Filters.Block = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_EnergyShield:
                    filters.Filters.EnergyShield = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Evasion:
                    filters.Filters.Evasion = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;
            }
        }

        return hasValue ? filters : null;
    }

    private MapFilterGroup? GetMapFilters(List<PropertyFilter> propertyFilters)
    {
        var filters = new MapFilterGroup();
        var hasValue = false;

        foreach (var propertyFilter in propertyFilters)
        {
            if (propertyFilter.Checked != true)
            {
                continue;
            }

            switch (propertyFilter.Type)
            {
                case PropertyFilterType.Map_ItemQuantity:
                    filters.Filters.ItemQuantity = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Map_ItemRarity:
                    filters.Filters.ItemRarity = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Map_AreaLevel:
                    filters.Filters.AreaLevel = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Map_MonsterPackSize:
                    filters.Filters.MonsterPackSize = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Map_Blighted:
                    filters.Filters.Blighted = new SearchFilterOption(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Map_BlightRavaged:
                    filters.Filters.BlightRavavaged = new SearchFilterOption(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Map_Tier:
                    filters.Filters.MapTier = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;
            }
        }

        return hasValue ? filters : null;
    }

    private MiscFilterGroup? GetMiscFilters(Item item, List<PropertyFilter> propertyFilters)
    {
        var filters = new MiscFilterGroup();
        var hasValue = false;

        foreach (var propertyFilter in propertyFilters)
        {
            if (propertyFilter.Type == PropertyFilterType.Misc_Corrupted)
            {
                filters.Filters.Corrupted = propertyFilter.Checked.HasValue ? new SearchFilterOption(propertyFilter) : null;
                hasValue = filters.Filters.Corrupted != null;
                break;
            }

            if (propertyFilter.Checked != true)
            {
                continue;
            }

            switch (propertyFilter.Type)
            {
                // Influence
                case PropertyFilterType.Misc_Influence_Crusader:
                    filters.Filters.CrusaderItem = new SearchFilterOption(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_Influence_Elder:
                    filters.Filters.ElderItem = new SearchFilterOption(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_Influence_Hunter:
                    filters.Filters.HunterItem = new SearchFilterOption(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_Influence_Redeemer:
                    filters.Filters.RedeemerItem = new SearchFilterOption(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_Influence_Shaper:
                    filters.Filters.ShaperItem = new SearchFilterOption(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_Influence_Warlord:
                    filters.Filters.WarlordItem = new SearchFilterOption(propertyFilter);
                    hasValue = true;
                    break;

                // Misc
                case PropertyFilterType.Misc_Quality:
                    filters.Filters.Quality = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_GemLevel:
                    if (apiInvariantItemProvider.UncutGemIds.Contains(item.Header.ApiItemId))
                    {
                        filters.Filters.ItemLevel = new StatFilterValue(propertyFilter);
                    }
                    else
                    {
                        filters.Filters.GemLevel = new StatFilterValue(propertyFilter);
                    }

                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_ItemLevel:
                    filters.Filters.ItemLevel = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_Scourged:
                    filters.Filters.Scourged = new StatFilterValue(propertyFilter);
                    hasValue = true;
                    break;
            }
        }

        return hasValue ? filters : null;
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
            ItemLevel = result.Item?.ItemLevel ?? 0,
            Corrupted = result.Item?.Corrupted ?? false,
            Identified = result.Item?.Identified ?? false,
            Armor = result.Item?.Extended?.ArmourAtMax ?? 0,
            EnergyShield = result.Item?.Extended?.EnergyShieldAtMax ?? 0,
            Evasion = result.Item?.Extended?.EvasionAtMax ?? 0,
            TotalDps = result.Item?.Extended?.DamagePerSecond ?? 0,
            ElementalDps = result.Item?.Extended?.ElementalDps ?? 0,
            PhysicalDps = result.Item?.Extended?.PhysicalDps ?? 0,
            BaseDefencePercentile = result.Item?.Extended?.BaseDefencePercentile,
        };

        var influences = result.Item?.Influences ?? new();

        var item = new TradeItem(itemHeader: header,
                                 itemProperties: properties,
                                 influences: influences,
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
