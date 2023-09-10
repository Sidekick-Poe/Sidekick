using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade
{
    public class TradeSearchService : ITradeSearchService
    {
        private readonly ILogger logger;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly ISettings settings;
        private readonly IPoeTradeClient poeTradeClient;
        private readonly IItemStaticDataProvider itemStaticDataProvider;
        private readonly IModifierProvider modifierProvider;

        public TradeSearchService(ILogger<TradeSearchService> logger,
            IGameLanguageProvider gameLanguageProvider,
            ISettings settings,
            IPoeTradeClient poeTradeClient,
            IItemStaticDataProvider itemStaticDataProvider,
            IModifierProvider modifierProvider)
        {
            this.logger = logger;
            this.gameLanguageProvider = gameLanguageProvider;
            this.settings = settings;
            this.poeTradeClient = poeTradeClient;
            this.itemStaticDataProvider = itemStaticDataProvider;
            this.modifierProvider = modifierProvider;
        }

        public async Task<TradeSearchResult<string>> SearchBulk(Item item)
        {
            try
            {
                logger.LogInformation("[Trade API] Querying Exchange API.");

                if (gameLanguageProvider.Language == null)
                {
                    throw new Exception("[Trade API] Could not find a valid language.");
                }

                var uri = $"{gameLanguageProvider.Language.PoeTradeApiBaseUrl}exchange/{settings.LeagueId}";

                var itemId = itemStaticDataProvider.GetId(item);
                if (itemId == null)
                {
                    throw new Exception("[Trade API] Could not find a valid item.");
                }

                var model = new BulkQueryRequest();
                model.Exchange.Want.Add(itemId);
                model.Exchange.Have.Add("chaos");

                var json = JsonSerializer.Serialize(model, poeTradeClient.Options);
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
                    return new() { Error = errorResult?.Error };
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

            throw new Exception("[Trade API] Could not understand the API response.");
        }

        public async Task<TradeSearchResult<string>> Search(Item item, PropertyFilters? propertyFilters = null, List<ModifierFilter>? modifierFilters = null)
        {
            try
            {
                logger.LogInformation("[Trade API] Querying Trade API.");

                if (gameLanguageProvider.Language == null)
                {
                    throw new Exception("[Trade API] Could not find a valid language.");
                }

                var request = new QueryRequest();

                if (item.Metadata.Category == Category.ItemisedMonster)
                {
                    if (!string.IsNullOrEmpty(item.Metadata.Name))
                    {
                        request.Query.Term = item.Metadata.Name;
                    }
                    else if (!string.IsNullOrEmpty(item.Metadata.Type))
                    {
                        request.Query.Type = item.Metadata.Type;
                    }
                }
                else if (item.Metadata.Rarity == Rarity.Unique)
                {
                    request.Query.Name = item.Metadata.Name;
                    request.Query.Type = item.Metadata.Type;

                    var rarity = item.Properties.IsRelic ? "uniquefoil" : "Unique";
                    request.Query.Filters.TypeFilters.Filters.Rarity = new SearchFilterOption(rarity);
                }
                else
                {
                    request.Query.Type = item.Metadata.Type;
                    request.Query.Filters.TypeFilters.Filters.Rarity = new SearchFilterOption("nonunique");
                }

                SetPropertyFilters(request.Query, propertyFilters);
                SetModifierFilters(request.Query.Stats, modifierFilters);
                SetSocketFilters(item, request.Query.Filters);

                if (item.Properties.AlternateQuality)
                {
                    request.Query.Term = item.Original.Name;
                }

                var uri = new Uri($"{gameLanguageProvider.Language.PoeTradeApiBaseUrl}search/{settings.LeagueId}");
                var json = JsonSerializer.Serialize(request, poeTradeClient.Options);
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

                    return new() { Error = errorResult?.Error };
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

            throw new Exception("[Trade API] Could not understand the API response.");
        }

        private static void SetPropertyFilters(Query query, PropertyFilters? propertyFilters)
        {
            if (propertyFilters == null)
            {
                return;
            }

            if (propertyFilters.Class.HasValue && propertyFilters.Class.Value != Class.Undefined)
            {
                var category = propertyFilters.Class.Value switch
                {
                    Class.AbyssJewel => "jewel.abyss",
                    Class.ActiveSkillGems => "gem.activegem",
                    Class.Amulet => "accessory.amulet",
                    Class.Belt => "accessory.belt",
                    Class.Blueprint => "heistmission.blueprint",
                    Class.BodyArmours => "armour.chest",
                    Class.Boots => "armour.boots",
                    Class.Bows => "weapon.bow",
                    Class.Claws => "weapon.claw",
                    Class.Contract => "heistmission.contract",
                    Class.CriticalUtilityFlasks => "",
                    Class.Daggers => "weapon.dagger",
                    Class.DelveStackableSocketableCurrency => "currency.resonator",
                    Class.DivinationCard => "card",
                    Class.Gloves => "armour.gloves",
                    Class.HeistBrooch => "heistequipment.heistreward",
                    Class.HeistCloak => "heistequipment.heistutility",
                    Class.HeistGear => "heistequipment.heistweapon",
                    Class.HeistTarget => "currency.heistobjective",
                    Class.HeistTool => "heistequipment.heisttool",
                    Class.Helmets => "armour.helmet",
                    Class.HybridFlasks => "flask",
                    Class.Jewel => "jewel.base",
                    Class.LifeFlasks => "flask",
                    Class.Logbooks => "logbook",
                    Class.ManaFlasks => "flask",
                    Class.MapFragments => "map.fragment",
                    // Maven invitations are in misc map items class at the moment. Ignoring for now.
                    // Class.MapInvitations => "map.invitation",
                    // This class does not exist, though the filter does. Ignoring for now.
                    // Class.MapScarabs => "map.scarab",
                    Class.Maps => "map",
                    Class.MetamorphSample => "monster.sample",
                    // Ignoring for now
                    // Class.MiscMapItems => "",
                    Class.OneHandAxes => "weapon.oneaxe",
                    Class.OneHandMaces => "weapon.onemace",
                    Class.OneHandSwords => "weapon.onesword",
                    Class.Quivers => "armour.quiver",
                    Class.Ring => "accessory.ring",
                    Class.RuneDaggers => "weapon.runedagger",
                    Class.Sceptres => "weapon.sceptre",
                    Class.Shields => "armour.shield",
                    // There are a lot of other uses for stackable currency currently such as beasts and scarabs. Ignoring for now.
                    // Class.StackableCurrency => "currency",
                    Class.Staves => "weapon.staff",
                    Class.SupportSkillGems => "gem.supportgem",
                    Class.ThrustingOneHandSwords => "",
                    Class.Trinkets => "accessory.trinket",
                    Class.TwoHandAxes => "weapon.twoaxe",
                    Class.TwoHandMaces => "weapon.twomace",
                    Class.TwoHandSwords => "weapon.twosword",
                    Class.UtilityFlasks => "flask",
                    Class.Wands => "weapon.wand",
                    Class.Warstaves => "weapon.warstaff",
                    Class.Sentinel => "sentinel",
                    Class.MemoryLine => "memoryline",
                    _ => null,
                };

                if (!string.IsNullOrEmpty(category))
                {
                    query.Filters.TypeFilters.Filters.Category = new SearchFilterOption(category);
                    query.Type = null;
                }
            }

            SetPropertyFilters(query.Filters, propertyFilters.Armour);
            SetPropertyFilters(query.Filters, propertyFilters.Weapon);
            SetPropertyFilters(query.Filters, propertyFilters.Map);
            SetPropertyFilters(query.Filters, propertyFilters.Misc);
        }

        private static void SetPropertyFilters(SearchFilters filters, List<PropertyFilter> propertyFilters)
        {
            foreach (var propertyFilter in propertyFilters)
            {
                if (!propertyFilter.Enabled == true && propertyFilter.Type != PropertyFilterType.Misc_Corrupted)
                {
                    continue;
                }

                switch (propertyFilter.Type)
                {
                    // Armour
                    case PropertyFilterType.Armour_Armour:
                        filters.ArmourFilters.Filters.Armor = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Armour_Block:
                        filters.ArmourFilters.Filters.Block = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Armour_EnergyShield:
                        filters.ArmourFilters.Filters.EnergyShield = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Armour_Evasion:
                        filters.ArmourFilters.Filters.Evasion = new SearchFilterValue(propertyFilter);
                        break;

                    // Category
                    case PropertyFilterType.Category:
                        filters.TypeFilters.Filters.Category = new SearchFilterOption(propertyFilter);
                        break;

                    // Influence
                    case PropertyFilterType.Misc_Influence_Crusader:
                        filters.MiscFilters.Filters.CrusaderItem = new SearchFilterOption(propertyFilter);
                        break;

                    case PropertyFilterType.Misc_Influence_Elder:
                        filters.MiscFilters.Filters.ElderItem = new SearchFilterOption(propertyFilter);
                        break;

                    case PropertyFilterType.Misc_Influence_Hunter:
                        filters.MiscFilters.Filters.HunterItem = new SearchFilterOption(propertyFilter);
                        break;

                    case PropertyFilterType.Misc_Influence_Redeemer:
                        filters.MiscFilters.Filters.RedeemerItem = new SearchFilterOption(propertyFilter);
                        break;

                    case PropertyFilterType.Misc_Influence_Shaper:
                        filters.MiscFilters.Filters.ShaperItem = new SearchFilterOption(propertyFilter);
                        break;

                    case PropertyFilterType.Misc_Influence_Warlord:
                        filters.MiscFilters.Filters.WarlordItem = new SearchFilterOption(propertyFilter);
                        break;

                    // Map
                    case PropertyFilterType.Map_ItemQuantity:
                        filters.MapFilters.Filters.ItemQuantity = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Map_ItemRarity:
                        filters.MapFilters.Filters.ItemRarity = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Map_AreaLevel:
                        filters.MapFilters.Filters.AreaLevel = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Map_MonsterPackSize:
                        filters.MapFilters.Filters.MonsterPackSize = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Map_Blighted:
                        filters.MapFilters.Filters.Blighted = new SearchFilterOption(propertyFilter);
                        break;

                    case PropertyFilterType.Map_BlightRavaged:
                        filters.MapFilters.Filters.BlightRavavaged = new SearchFilterOption(propertyFilter);
                        break;

                    case PropertyFilterType.Map_Tier:
                        filters.MapFilters.Filters.MapTier = new SearchFilterValue(propertyFilter);
                        break;

                    // Misc
                    case PropertyFilterType.Misc_Quality:
                        filters.MiscFilters.Filters.Quality = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Misc_GemLevel:
                        filters.MiscFilters.Filters.GemLevel = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Misc_ItemLevel:
                        filters.MiscFilters.Filters.ItemLevel = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Misc_Corrupted:
                        filters.MiscFilters.Filters.Corrupted = propertyFilter.Enabled == null || propertyFilter.Enabled == true ? new SearchFilterOption(propertyFilter) : null;
                        break;

                    case PropertyFilterType.Misc_Scourged:
                        filters.MiscFilters.Filters.Scourged = new SearchFilterValue(propertyFilter);
                        break;

                    // Weapon
                    case PropertyFilterType.Weapon_PhysicalDps:
                        filters.WeaponFilters.Filters.PhysicalDps = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Weapon_ElementalDps:
                        filters.WeaponFilters.Filters.ElementalDps = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Weapon_Dps:
                        filters.WeaponFilters.Filters.DamagePerSecond = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Weapon_AttacksPerSecond:
                        filters.WeaponFilters.Filters.AttacksPerSecond = new SearchFilterValue(propertyFilter);
                        break;

                    case PropertyFilterType.Weapon_CriticalStrikeChance:
                        filters.WeaponFilters.Filters.CriticalStrikeChance = new SearchFilterValue(propertyFilter);
                        break;
                }
            }
        }

        private static void SetModifierFilters(List<StatFilterGroup> stats, List<ModifierFilter>? modifierFilters)
        {
            if (modifierFilters == null)
            {
                return;
            }

            var group = new StatFilterGroup();
            group.Filters.AddRange(modifierFilters
                .Where(x => x.Line?.Modifier != null)
                .Select(x => new StatFilter()
                {
                    Disabled = !(x.Enabled ?? false),
                    Id = x.Line?.Modifier?.Id,
                    Value = new SearchFilterValue(x),
                })
                .ToList());

            stats.Add(group);
        }

        private static void SetSocketFilters(Item item, SearchFilters filters)
        {
            // Auto Search 5+ Links
            var highestCount = item.Sockets
                .GroupBy(x => x.Group)
                .Select(x => x.Count())
                .OrderByDescending(x => x)
                .FirstOrDefault();
            if (highestCount >= 5)
            {
                filters.SocketFilters.Filters.Links = new SocketFilterOption()
                {
                    Min = highestCount,
                };
            }
        }

        public async Task<List<TradeItem>> GetResults(string queryId, List<string> ids, List<ModifierFilter>? modifierFilters = null)
        {
            try
            {
                logger.LogInformation($"[Trade API] Fetching Trade API Listings from Query {queryId}.");

                if (gameLanguageProvider.Language == null)
                {
                    return new();
                }

                var pseudo = string.Empty;
                if (modifierFilters != null)
                {
                    pseudo = string.Join("", modifierFilters
                        .Where(x => x.Line?.Modifier != null && x.Line.Modifier.Category == ModifierCategory.Pseudo)
                        .Select(x => $"&pseudos[]={x.Line?.Modifier?.Id}"));
                }

                var response = await poeTradeClient.HttpClient.GetAsync(gameLanguageProvider.Language.PoeTradeApiBaseUrl + "fetch/" + string.Join(",", ids) + "?query=" + queryId + pseudo);
                if (!response.IsSuccessStatusCode)
                {
                    return new();
                }

                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<FetchResult<Result>>(content, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });
                if (result == null)
                {
                    return new();
                }

                return result.Result.Where(x => x != null).ToList().ConvertAll(x => GetItem(x));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"[Trade API] Exception thrown when fetching trade API listings from Query {queryId}.");
            }

            return new();
        }

        private TradeItem GetItem(Result result)
        {
            var metadata = new ItemMetadata()
            {
                Name = result.Item?.Name,
                Rarity = result.Item?.Rarity ?? Rarity.Unknown,
                Type = result.Item?.TypeLine,
            };

            var original = new OriginalItem()
            {
                Name = result.Item?.Name,
                Text = Encoding.UTF8.GetString(Convert.FromBase64String(result.Item?.Extended?.Text ?? string.Empty)),
                Type = result.Item?.TypeLine,
            };

            var properties = new Properties()
            {
                ItemLevel = result.Item?.ItemLevel ?? 0,
                Corrupted = result.Item?.Corrupted ?? false,
                Scourged = result.Item?.Scourged.Tier != 0,
                IsRelic = result.Item?.IsRelic ?? false,
                Identified = result.Item?.Identified ?? false,
                Armor = result.Item?.Extended?.ArmourAtMax ?? 0,
                EnergyShield = result.Item?.Extended?.EnergyShieldAtMax ?? 0,
                Evasion = result.Item?.Extended?.EvasionAtMax ?? 0,
                DamagePerSecond = result.Item?.Extended?.DamagePerSecond ?? 0,
                ElementalDps = result.Item?.Extended?.ElementalDps ?? 0,
                PhysicalDps = result.Item?.Extended?.PhysicalDps ?? 0,
                BaseDefencePercentile = result.Item?.Extended?.BaseDefencePercentile,
            };

            var influences = result.Item?.Influences ?? new();

            var item = new TradeItem(
                    metadata: metadata,
                    original: original,
                    properties: properties,
                    influences: influences,
                    sockets: ParseSockets(result.Item?.Sockets).ToList(),
                    modifierLines: new(),
                    pseudoModifiers: new())
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

            item.ModifierLines.AddRange(ParseModifierLines(
                result.Item?.EnchantMods,
                result.Item?.Extended?.Mods?.Enchant,
                ParseHash(result.Item?.Extended?.Hashes?.Enchant)));

            item.ModifierLines.AddRange(ParseModifierLines(
                result.Item?.ImplicitMods ?? result.Item?.LogbookMods.SelectMany(x => x.Mods).ToList(),
                result.Item?.Extended?.Mods?.Implicit,
                ParseHash(result.Item?.Extended?.Hashes?.Implicit)));

            item.ModifierLines.AddRange(ParseModifierLines(
                result.Item?.CraftedMods,
                result.Item?.Extended?.Mods?.Crafted,
                ParseHash(result.Item?.Extended?.Hashes?.Crafted)));

            item.ModifierLines.AddRange(ParseModifierLines(
                result.Item?.ExplicitMods,
                result.Item?.Extended?.Mods?.Explicit,
                ParseHash(result.Item?.Extended?.Hashes?.Explicit, result.Item?.Extended?.Hashes?.Monster)));

            item.ModifierLines.AddRange(ParseModifierLines(
                result.Item?.FracturedMods,
                result.Item?.Extended?.Mods?.Fractured,
                ParseHash(result.Item?.Extended?.Hashes?.Fractured)));

            item.ModifierLines.AddRange(ParseModifierLines(
                result.Item?.ScourgeMods,
                result.Item?.Extended?.Mods?.Scourge,
                ParseHash(result.Item?.Extended?.Hashes?.Scourge)));

            item.PseudoModifiers.AddRange(ParseModifiers(
                result.Item?.PseudoMods,
                result.Item?.Extended?.Mods?.Pseudo,
                ParseHash(result.Item?.Extended?.Hashes?.Pseudo)));

            item.ModifierLines = item.ModifierLines
                .OrderBy(x => item.Original.Text?.IndexOf(x.Text ?? string.Empty))
                .ToList();

            return item;
        }

        private static List<LineContentValue> ParseHash(params List<List<JsonElement>>?[] hashes)
        {
            var result = new List<LineContentValue>();

            foreach (var values in hashes)
            {
                if (values != null)
                {
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
            }

            return result;
        }

        private static List<LineContent> ParseLineContents(List<ResultLineContent>? lines, bool executeOrderBy = true)
        {
            if (lines == null)
            {
                return new();
            }

            return lines
                .OrderBy(x => executeOrderBy ? x.Order : 0)
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

                    if (values.Count > 0)
                    {
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

                            case 1:
                                text = $"{values[0].Value} {line.Name}";
                                break;

                            case 2:
                                text = $"{values[0].Value}";
                                break;

                            case 3:
                                var format = Regex.Replace(line.Name ?? string.Empty, "%(\\d)", "{$1}");
                                text = string.Format(format, values.Select(x => x.Value).ToArray());
                                break;

                            default:
                                text = $"{line.Name} {string.Join(", ", values.Select(x => x.Value))}";
                                break;
                        }
                    }

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
                if (id == null)
                {
                    continue;
                }

                var text = texts.FirstOrDefault(x => modifierProvider.IsMatch(id, x)) ?? texts[index];
                var mod = mods.FirstOrDefault(x => x.Magnitudes != null && x.Magnitudes.Any(y => y.Hash == id));

                yield return new ModifierLine(
                    text: text)
                {
                    Modifier = new Modifier(
                        text: text)
                    {
                        Id = id,
                        Category = modifierProvider.GetModifierCategory(id),
                        Tier = mod?.Tier,
                        TierName = mod?.Name,
                    },
                };
            }
        }

        private IEnumerable<Modifier> ParseModifiers(List<string>? texts, List<Mod>? mods, List<LineContentValue>? hashes)
        {
            if (texts == null || mods == null || hashes == null)
            {
                yield break;
            }

            for (var index = 0; index < hashes.Count; index++)
            {
                var id = hashes[index].Value;
                if (id == null)
                {
                    continue;
                }

                var text = texts.FirstOrDefault(x => modifierProvider.IsMatch(id, x));
                if (text == null)
                {
                    continue;
                }

                var mod = mods.FirstOrDefault(x => x.Magnitudes != null && x.Magnitudes.Any(y => y.Hash == id));
                yield return new Modifier(
                    text: text)
                {
                    Id = id,
                    Category = modifierProvider.GetModifierCategory(id),
                    Tier = mod?.Tier,
                    TierName = mod?.Name,
                };
            }
        }

        private static IEnumerable<Socket> ParseSockets(List<ResultSocket>? sockets)
        {
            if (sockets == null)
            {
                return Enumerable.Empty<Socket>();
            }

            return sockets
                .Where(x => x.ColourString != "DV") // Remove delve resonator sockets
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
                        _ => throw new Exception("Invalid socket"),
                    }
                });
        }

        public Uri GetTradeUri(Item item, string queryId)
        {
            Uri? baseUri;
            if (item.Metadata.Rarity == Rarity.Currency && itemStaticDataProvider.GetId(item) != null)
            {
                baseUri = gameLanguageProvider.Language?.PoeTradeExchangeBaseUrl;
            }
            else
            {
                baseUri = gameLanguageProvider.Language?.PoeTradeSearchBaseUrl;
            }

            if (baseUri == null)
            {
                throw new Exception("[Trade API] Could not find the trade uri.");
            }

            return new Uri(baseUri, $"{settings.LeagueId}/{queryId}");
        }
    }
}
