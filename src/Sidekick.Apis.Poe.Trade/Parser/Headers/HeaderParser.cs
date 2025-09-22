using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Items.Models;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Sidekick.Common.Exceptions;

namespace Sidekick.Apis.Poe.Trade.Parser.Headers;

public class HeaderParser
(
    IGameLanguageProvider gameLanguageProvider,
    IApiItemProvider apiItemProvider,
    IApiInvariantItemProvider apiInvariantItemProvider,
    IPropertyParser propertyParser
) : IHeaderParser
{
    public int Priority => 200;

    private Regex Affixes { get; set; } = null!;

    private Regex SuperiorAffix { get; set; } = null!;

    private string GetLineWithoutAffixes(string line) => Affixes.Replace(line, string.Empty).Trim(' ', ',');

    private string GetLineWithoutSuperiorAffix(string line) => SuperiorAffix.Replace(line, string.Empty).Trim(' ', ',');

    public Task Initialize()
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

        Affixes = new Regex("(?:" + GetRegexLine(gameLanguageProvider.Language.AffixSuperior) + "|" + GetRegexLine(gameLanguageProvider.Language.AffixBlighted) + "|" + GetRegexLine(gameLanguageProvider.Language.AffixBlightRavaged) + ")");
        SuperiorAffix = new Regex("(?:" + GetRegexLine(gameLanguageProvider.Language.AffixSuperior) + ")");

        return Task.CompletedTask;
    }

    public void Parse(Item item)
    {
        propertyParser.GetDefinition<ItemClassProperty>().Parse(item);

        var apiItem = GetApiItem(item);
        if (apiItem != null)
        {
            var categoryRarity = apiItem.Category switch
            {
                Category.DivinationCard => Rarity.DivinationCard,
                Category.Gem => Rarity.Gem,
                Category.Currency => Rarity.Currency,
                _ => Rarity.Unknown,
            };

            item.Header.ApiItemId = apiItem.Id;
            item.Header.ApiName = apiItem.Name;
            item.Header.ApiType = apiItem.Type;
            item.Header.ApiDiscriminator = apiItem.Discriminator;
            item.Header.ApiText = apiItem.Text;
            item.Header.Category = apiItem.Category;

            if(apiItem.IsUnique) item.Properties.Rarity = Rarity.Unique;
            else if (categoryRarity != Rarity.Unknown) item.Properties.Rarity = categoryRarity;
        }

        if (string.IsNullOrEmpty(item.Header.ApiName) && string.IsNullOrEmpty(item.Header.ApiType))
        {
            throw new UnparsableException(item.Text.Text);
        }

        ParseVaalGem(item);

        item.Text.Blocks[0].Parsed = true;
    }

    private ApiItem? GetApiItem(Item item)
    {
        if (item.Properties.ItemClass == ItemClass.UncutSkillGem && apiItemProvider.IdDictionary.TryGetValue(apiInvariantItemProvider.UncutSkillGemId, out var uncutSkillGem))
        {
            return uncutSkillGem;
        }

        if (item.Properties.ItemClass == ItemClass.UncutSupportGem && apiItemProvider.IdDictionary.TryGetValue(apiInvariantItemProvider.UncutSupportGemId, out var uncutSupportGem))
        {
            return uncutSupportGem;
        }

        if (item.Properties.ItemClass == ItemClass.UncutSpiritGem && apiItemProvider.IdDictionary.TryGetValue(apiInvariantItemProvider.UncutSpiritGemId, out var uncutSpiritGem))
        {
            return uncutSpiritGem;
        }

        // Rares may have conflicting names, so we don't want to search any unique items that may have that name. Like "Ancient Orb" which can be used by abyss jewels.
        var name = item.Properties.Rarity is Rarity.Rare or Rarity.Magic ? null : item.Name;
        name = name != null ? GetLineWithoutSuperiorAffix(name) : null;
        var type = item.Type != null ? GetLineWithoutSuperiorAffix(item.Type) : null;

        // We can find multiple matches while parsing. This will store all of them. We will figure out which result is correct at the end of this method.
        var results = new List<ApiItem>();

        // There are some items which have prefixes which we don't want to remove, like the "Blighted Delirium Orb".
        if (!string.IsNullOrEmpty(name) && apiItemProvider.NameAndTypeDictionary.TryGetValue(name, out var itemData))
        {
            results.AddRange(itemData);
        }

        // Here we check without any prefixes
        else if (!string.IsNullOrEmpty(name) && apiItemProvider.NameAndTypeDictionary.TryGetValue(GetLineWithoutAffixes(name), out itemData))
        {
            results.AddRange(itemData);
        }

        // Now we check the type
        else if (!string.IsNullOrEmpty(type) && apiItemProvider.NameAndTypeDictionary.TryGetValue(type, out itemData))
        {
            results.AddRange(itemData);
        }

        else if (!string.IsNullOrEmpty(type) && apiItemProvider.NameAndTypeDictionary.TryGetValue(GetLineWithoutAffixes(type), out itemData))
        {
            results.AddRange(itemData);
        }

        // Finally. if we don't have any matches, we will look into our regex dictionary
        else
        {
            if (!string.IsNullOrEmpty(name))
            {
                results.AddRange(apiItemProvider.NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(name)).Select(x => x.Item));
            }

            if (!string.IsNullOrEmpty(type))
            {
                results.AddRange(apiItemProvider.NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(type)).Select(x => x.Item));
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

    private void ParseVaalGem(Item item)
    {
        var canBeVaalGem = item.Properties.ItemClass == ItemClass.ActiveGem && item.Text.Blocks.Count > 7;
        if (!canBeVaalGem || item.Text.Blocks[5].Lines.Count <= 0) return;

        if (!apiItemProvider.NameAndTypeDictionary.TryGetValue(item.Text.Blocks[5].Lines[0].Text, out var apiItems)) return;

        var apiItem = apiItems.First();
        item.Header.ApiItemId = apiItem.Id;
        item.Header.ApiName = apiItem.Name;
        item.Header.ApiType = apiItem.Type;
        item.Header.ApiDiscriminator = apiItem.Discriminator;
        item.Header.ApiText = apiItem.Text;
        item.Header.Category = apiItem.Category;

    }
}

