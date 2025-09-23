using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Apis.Poe.Trade.Items.Models;

namespace Sidekick.Apis.Poe.Trade.Items;

public abstract class BaseItemProvider(ILogger logger)
{
    public Dictionary<string, List<ItemApiInformation>> NameAndTypeDictionary { get; } = new();

    public List<(Regex Regex, ItemApiInformation Item)> NameAndTypeRegex { get; } = new();

    private Regex Affixes { get; set; } = null!;

    private Regex SuperiorAffix { get; set; } = null!;

    private string GetLineWithoutAffixes(string line) => Affixes.Replace(line, string.Empty).Trim(' ', ',');

    private string GetLineWithoutSuperiorAffix(string line) => SuperiorAffix.Replace(line, string.Empty).Trim(' ', ',');

    public Task InitializeLanguage(IGameLanguage language)
    {
        Regex GetRegexLine(string input)
        {
            if (input.StartsWith('/'))
            {
                input = input.Trim('/');
                return new Regex($"^{input} | {input}$");
            }

            input = Regex.Escape(input);
            return new Regex($"^{input} | {input}$");
        }

        Affixes = new Regex("(?:" + GetRegexLine(language.AffixSuperior) + "|" + GetRegexLine(language.AffixBlighted) + "|" + GetRegexLine(language.AffixBlightRavaged) + ")");
        SuperiorAffix = new Regex("(?:" + GetRegexLine(language.AffixSuperior) + ")");

        return Task.CompletedTask;
    }

    protected void InitializeItems(GameType game, FetchResult<ApiCategory> result){
        NameAndTypeDictionary.Clear();
        NameAndTypeRegex.Clear();

        var categories = game switch
        {
            GameType.PathOfExile2 => ApiItemConstants.Poe2Categories,
            _ => ApiItemConstants.Poe1Categories,
        };

        foreach (var category in categories)
        {
            FillCategoryItems(result.Result, category.Key, category.Value.Category, category.Value.UseRegex);
        }
    }

    private void FillCategoryItems(List<ApiCategory> categories, string categoryId, Category category, bool useRegex = false)
    {
        var categoryItems = categories.SingleOrDefault(x => x.Id == categoryId);
        if (categoryItems == null)
        {
            logger.LogWarning($"[MetadataProvider] The category '{categoryId}' could not be found in the metadata from the API.");
            return;
        }

        foreach (var entry in categoryItems.Entries)
        {
            var information = entry.ToItemApiInformation();
            information.Category = category;

            if (!string.IsNullOrEmpty(information.Name))
            {
                FillDictionary(information.Name, information);
                if (!information.IsUnique && useRegex) NameAndTypeRegex.Add((new Regex(Regex.Escape(information.Name)), information));
            }

            if (!string.IsNullOrEmpty(information.Type))
            {
                FillDictionary(information.Type, information);
                if (!information.IsUnique && useRegex) NameAndTypeRegex.Add((new Regex(Regex.Escape(information.Type)), information));
            }

            if (!string.IsNullOrEmpty(information.Text))
            {
                FillDictionary(information.Text, information);
                if (!information.IsUnique && useRegex) NameAndTypeRegex.Add((new Regex(Regex.Escape(information.Text)), information));
            }
        }
    }

    private void FillDictionary(string key, ItemApiInformation metadata)
    {
        if (!NameAndTypeDictionary.TryGetValue(key, out var dictionaryEntry))
        {
            dictionaryEntry = new List<ItemApiInformation>();
            NameAndTypeDictionary.Add(key, dictionaryEntry);
        }

        dictionaryEntry.Add(metadata);
    }

    public ItemApiInformation? GetApiItem(Rarity rarity, ItemClass itemClass, string? name, string? type)
    {
        if (itemClass is ItemClass.UncutSkillGem or ItemClass.UncutSpiritGem or ItemClass.UncutSupportGem)
        {
            return itemClass switch
            {
                ItemClass.UncutSkillGem when this is IApiInvariantItemProvider apiInvariantItemProvider => apiInvariantItemProvider.UncutSkillGem,
                ItemClass.UncutSpiritGem when this is IApiInvariantItemProvider apiInvariantItemProvider => apiInvariantItemProvider.UncutSpiritGem,
                ItemClass.UncutSupportGem when this is IApiInvariantItemProvider apiInvariantItemProvider => apiInvariantItemProvider.UncutSupportGem,
                _ => null,
            };
        }

        // Rares may have conflicting names, so we don't want to search any unique items that may have that name. Like "Ancient Orb" which can be used by abyss jewels.
        name = rarity is Rarity.Rare or Rarity.Magic ? null : name;
        name = name != null ? GetLineWithoutSuperiorAffix(name) : null;
        type = type != null ? GetLineWithoutSuperiorAffix(type) : null;

        // We can find multiple matches while parsing. This will store all of them. We will figure out which result is correct at the end of this method.
        var results = new List<ItemApiInformation>();

        // There are some items which have prefixes which we don't want to remove, like the "Blighted Delirium Orb".
        if (!string.IsNullOrEmpty(name) && NameAndTypeDictionary.TryGetValue(name, out var itemData))
        {
            results.AddRange(itemData);
        }

        // Here we check without any prefixes
        else if (!string.IsNullOrEmpty(name) && NameAndTypeDictionary.TryGetValue(GetLineWithoutAffixes(name), out itemData))
        {
            results.AddRange(itemData);
        }

        // Now we check the type
        else if (!string.IsNullOrEmpty(type) && NameAndTypeDictionary.TryGetValue(type, out itemData))
        {
            results.AddRange(itemData);
        }

        else if (!string.IsNullOrEmpty(type) && NameAndTypeDictionary.TryGetValue(GetLineWithoutAffixes(type), out itemData))
        {
            results.AddRange(itemData);
        }

        // Finally. if we don't have any matches, we will look into our regex dictionary
        else
        {
            if (!string.IsNullOrEmpty(name))
            {
                results.AddRange(NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(name)).Select(x => x.Item));
            }

            if (!string.IsNullOrEmpty(type))
            {
                results.AddRange(NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(type)).Select(x => x.Item));
            }
        }

        var orderedResults = results.OrderByDescending(x => x.Type?.Length ?? x.Name?.Length ?? 0).ToList();

        if (orderedResults.Any(x => x.Type == type))
        {
            return orderedResults.FirstOrDefault(x => x.Type == type);
        }

        if (orderedResults.Any(x => x.IsUnique))
        {
            return orderedResults.FirstOrDefault(x => x.IsUnique);
        }

        return orderedResults.FirstOrDefault();
    }
}
