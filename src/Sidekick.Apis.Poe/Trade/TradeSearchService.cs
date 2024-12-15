using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Requests.Models;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Enums;
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
    IModifierProvider modifierProvider
) : ITradeSearchService
{
    private readonly ILogger logger = logger;

    public async Task<TradeSearchResult<string>> Search(Item item, TradeCurrency currency, PropertyFilters? propertyFilters = null, List<ModifierFilter>? modifierFilters = null, List<PseudoModifierFilter>? pseudoFilters = null)
    {
        try
        {
            logger.LogInformation("[Trade API] Querying Trade API.");

            var query = new Query();
            if (propertyFilters?.BaseTypeFilterApplied ?? true)
            {
                var hasTypeDiscriminator = !string.IsNullOrEmpty(item.Metadata.ApiTypeDiscriminator);
                if (hasTypeDiscriminator)
                {
                    query.Type = new TypeDiscriminator()
                    {
                        Option = item.Metadata.ApiType,
                        Discriminator = item.Metadata.ApiTypeDiscriminator,
                    };
                }
                else if (!string.IsNullOrEmpty(item.Header.ItemCategory))
                {
                    query.Type = item.Metadata.ApiType;
                }
            }
            else if (propertyFilters.ClassFilterApplied)
            {
                query.Filters.TypeFilters.Filters.Category = GetCategoryFilter(item.Header.ItemCategory);
            }

            if (item.Metadata.Category == Category.ItemisedMonster)
            {
                if (!string.IsNullOrEmpty(item.Metadata.Name))
                {
                    query.Term = item.Metadata.Name;
                    query.Type = null;
                }
            }
            else if (item.Metadata.Rarity == Rarity.Unique)
            {
                query.Name = item.Metadata.Name;
                query.Filters.TypeFilters.Filters.Rarity = new SearchFilterOption("Unique");
            }
            else if (propertyFilters?.RarityFilterApplied ?? false)
            {
                var rarity = item.Metadata.Rarity switch
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

            var currencyValue = currency.GetValueAttribute();
            if (!string.IsNullOrEmpty(currencyValue))
            {
                query.Filters.TradeFilters = new TradeFilterGroup
                {
                    Filters =
                    {
                        Price = new SearchFilterValue(currencyValue),
                    },
                };
            }

            SetModifierFilters(query.Stats, modifierFilters);
            SetPseudoModifierFilters(query.Stats, pseudoFilters);
            SetSocketFilters(item, query.Filters);

            if (propertyFilters != null)
            {
                query.Filters.EquipmentFilters = GetEquipmentFilters(item, propertyFilters.Weapon.Concat(propertyFilters.Armour).ToList());
                query.Filters.WeaponFilters = GetWeaponFilters(item, propertyFilters.Weapon);
                query.Filters.ArmourFilters = GetArmourFilters(item, propertyFilters.Armour);
                query.Filters.MapFilters = GetMapFilters(propertyFilters.Map);
                query.Filters.MiscFilters = GetMiscFilters(item, propertyFilters.Misc);
            }

            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var uri = new Uri($"{gameLanguageProvider.Language.GetTradeApiBaseUrl(item.Metadata.Game)}search/{leagueId.GetUrlSlugForLeague()}");

            var json = JsonSerializer.Serialize(new QueryRequest()
                                                {
                                                    Query = query,
                                                },
                                                poeTradeClient.Options);

            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await poeTradeClient.HttpClient.PostAsync(uri, body);

            var content = await response.Content.ReadAsStreamAsync();
            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = await response.Content.ReadAsStringAsync();
                logger.LogWarning("[Trade API] Querying failed: {responseCode} {responseMessage}", response.StatusCode, responseMessage);
                logger.LogWarning("[Trade API] Uri: {uri}", uri);
                logger.LogWarning("[Trade API] Query: {query}", json);

                var errorResult = await JsonSerializer.DeserializeAsync<ErrorResult>(content, poeTradeClient.Options);
                throw new ApiErrorException("[Trade API] Querying failed. " + errorResult?.Error?.Message);
            }

            var result = await JsonSerializer.DeserializeAsync<TradeSearchResult<string>?>(content, poeTradeClient.Options);
            if (result != null)
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[Trade API] Exception thrown while querying trade api.");
        }

        throw new ApiErrorException("[Trade API] Could not understand the API response.");
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
        if (item.Metadata.Game == GameType.PathOfExile2)
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
                    filters.Filters.Damage = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_PhysicalDps:
                    filters.Filters.PhysicalDps = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_ElementalDps:
                    filters.Filters.ElementalDps = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_Dps:
                    filters.Filters.DamagePerSecond = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_AttacksPerSecond:
                    filters.Filters.AttacksPerSecond = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_CriticalStrikeChance:
                    filters.Filters.CriticalStrikeChance = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;
            }
        }

        return hasValue ? filters : null;
    }

    private ArmourFilterGroup? GetArmourFilters(Item item, List<PropertyFilter> propertyFilters)
    {
        if (item.Metadata.Game == GameType.PathOfExile2)
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
                    filters.Filters.Armor = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Block:
                    filters.Filters.Block = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_EnergyShield:
                    filters.Filters.EnergyShield = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Evasion:
                    filters.Filters.Evasion = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;
            }
        }

        return hasValue ? filters : null;
    }

    private EquipmentFilterGroup? GetEquipmentFilters(Item item, List<PropertyFilter> propertyFilters)
    {
        if (item.Metadata.Game == GameType.PathOfExile)
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
                    filters.Filters.Damage = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_PhysicalDps:
                    filters.Filters.PhysicalDps = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_ElementalDps:
                    filters.Filters.ElementalDps = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_Dps:
                    filters.Filters.DamagePerSecond = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_AttacksPerSecond:
                    filters.Filters.AttacksPerSecond = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Weapon_CriticalStrikeChance:
                    filters.Filters.CriticalStrikeChance = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Armour:
                    filters.Filters.Armor = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Block:
                    filters.Filters.Block = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_EnergyShield:
                    filters.Filters.EnergyShield = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Armour_Evasion:
                    filters.Filters.Evasion = new SearchFilterValue(propertyFilter);
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
                    filters.Filters.ItemQuantity = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Map_ItemRarity:
                    filters.Filters.ItemRarity = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Map_AreaLevel:
                    filters.Filters.AreaLevel = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Map_MonsterPackSize:
                    filters.Filters.MonsterPackSize = new SearchFilterValue(propertyFilter);
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
                    filters.Filters.MapTier = new SearchFilterValue(propertyFilter);
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

        if (item.Properties.Anomalous)
        {
            filters.Filters.GemQualityType = new SearchFilterOption(SearchFilterOption.AlternateGemQualityOptions.Anomalous);
            hasValue = true;
        }

        if (item.Properties.Divergent)
        {
            filters.Filters.GemQualityType = new SearchFilterOption(SearchFilterOption.AlternateGemQualityOptions.Divergent);
            hasValue = true;
        }

        if (item.Properties.Phantasmal)
        {
            filters.Filters.GemQualityType = new SearchFilterOption(SearchFilterOption.AlternateGemQualityOptions.Phantasmal);
            hasValue = true;
        }

        foreach (var propertyFilter in propertyFilters)
        {
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
                    filters.Filters.Quality = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_GemLevel:
                    filters.Filters.GemLevel = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_ItemLevel:
                    filters.Filters.ItemLevel = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_Corrupted:
                    filters.Filters.Corrupted = propertyFilter.Checked.HasValue ? new SearchFilterOption(propertyFilter) : null;
                    hasValue = true;
                    break;

                case PropertyFilterType.Misc_Scourged:
                    filters.Filters.Scourged = new SearchFilterValue(propertyFilter);
                    hasValue = true;
                    break;
            }
        }

        return hasValue ? filters : null;
    }

    private static void SetModifierFilters(List<StatFilterGroup> stats, List<ModifierFilter>? modifierFilters)
    {
        if (modifierFilters == null)
        {
            return;
        }

        var andGroup = stats.FirstOrDefault(x => x.Type == StatType.And);
        if (andGroup == null)
        {
            andGroup = new StatFilterGroup()
            {
                Type = StatType.And,
            };
            stats.Add(andGroup);
        }

        var countGroup = stats.FirstOrDefault(x => x.Type == StatType.Count);
        if (countGroup == null)
        {
            countGroup = new StatFilterGroup()
            {
                Type = StatType.Count,
                Value = new SearchFilterValue()
                {
                    Min = 0,
                },
            };
            stats.Add(countGroup);
        }

        foreach (var filter in modifierFilters)
        {
            if (filter.Checked != true)
            {
                continue;
            }

            if (filter.Line.Modifiers.Count == 1)
            {
                var modifier = filter.Line.Modifiers.FirstOrDefault();
                if (modifier == null)
                {
                    continue;
                }

                andGroup.Filters.Add(new StatFilters()
                {
                    Id = modifier.Id,
                    Value = new SearchFilterValue(filter),
                });
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
                    Value = new SearchFilterValue(filter),
                });
            }

            if (countGroup.Value != null && modifiers.Any())
            {
                countGroup.Value.Min += 1;
            }
        }
    }

    private static void SetPseudoModifierFilters(List<StatFilterGroup> stats, List<PseudoModifierFilter>? pseudoFilters)
    {
        if (pseudoFilters == null)
        {
            return;
        }

        var andGroup = stats.FirstOrDefault(x => x.Type == StatType.And);
        if (andGroup == null)
        {
            andGroup = new StatFilterGroup()
            {
                Type = StatType.And,
            };
            stats.Add(andGroup);
        }

        foreach (var filter in pseudoFilters)
        {
            if (filter.Checked != true)
            {
                continue;
            }

            andGroup.Filters.Add(new StatFilters()
            {
                Id = filter.Modifier.Id,
                Value = new SearchFilterValue(filter),
            });
        }
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

            var pseudo = string.Empty;
            if (pseudoFilters?.Count > 0)
            {
                pseudo = string.Join("", pseudoFilters.Select(x => $"&pseudos[]={x.Modifier.Id}"));
            }

            var response = await poeTradeClient.HttpClient.GetAsync(gameLanguageProvider.Language.GetTradeApiBaseUrl(game) + "fetch/" + string.Join(",", ids) + "?query=" + queryId + pseudo);
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
        var metadata = new ItemMetadata()
        {
            Id = "",
            Name = result.Item?.Name,
            Rarity = result.Item?.Rarity ?? Rarity.Unknown,
            Type = result.Item?.TypeLine,
            ApiType = result.Item?.TypeLine,
            Category = Category.Unknown,
            Game = game,
        };

        var original = new Header()
        {
            Name = result.Item?.Name,
            Type = result.Item?.TypeLine,
        };

        var properties = new Properties()
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

        var item = new TradeItem(metadata: metadata,
                                 original: original,
                                 properties: properties,
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

        item.PseudoModifiers.AddRange(ParsePseudoModifiers(result.Item?.PseudoMods, result.Item?.Extended?.Mods?.Pseudo, ParseHash(result.Item?.Extended?.Hashes?.Pseudo)));

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

    private IEnumerable<PseudoModifier> ParsePseudoModifiers(List<string>? texts, List<Mod>? mods, List<LineContentValue>? hashes)
    {
        if (texts == null || mods == null || hashes == null)
        {
            yield break;
        }

        foreach (var hash in hashes)
        {
            var id = hash.Value;
            if (id == null)
            {
                continue;
            }

            var text = texts.FirstOrDefault(x => modifierProvider.IsMatch(id, x));
            if (text == null)
            {
                continue;
            }

            yield return new PseudoModifier(text: text)
            {
                Id = id,
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
        var baseUrl = gameLanguageProvider.Language.GetTradeBaseUrl(game);
        var baseUri = new Uri(baseUrl + "search/");
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        return new Uri(baseUri, $"{leagueId.GetUrlSlugForLeague()}/{queryId}");
    }
}
