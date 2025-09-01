using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Items;
using Sidekick.Apis.Poe.Trade.Items.Models;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Headers;

public class HeaderParser
(
    IGameLanguageProvider gameLanguageProvider,
    IApiItemProvider apiItemProvider,
    IApiInvariantItemProvider apiInvariantItemProvider,
    IRarityParser rarityParser,
    IItemClassParser itemClassParser
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

    public ItemHeader Parse(ParsingItem parsingItem)
    {
        var header = CreateItemHeader(parsingItem);

        var apiItem = GetApiItem(header);
        if (apiItem != null)
        {
            var apiHeader = apiItem.ToHeader();
            apiHeader.Name = header.Name;
            apiHeader.Type = header.Type;
            apiHeader.ItemClass = header.ItemClass;
            if (apiHeader.Rarity == Rarity.Unknown) apiHeader.Rarity = header.Rarity;

            header = apiHeader;
        }

        if (string.IsNullOrEmpty(header.ApiName) && string.IsNullOrEmpty(header.ApiType))
        {
            throw new UnparsableException(parsingItem.Text);
        }

        parsingItem.Blocks[0].Parsed = true;
        return header;
    }

    private ItemHeader CreateItemHeader(ParsingItem parsingItem)
    {
        var rarity = rarityParser.Parse(parsingItem);
        var itemClass = itemClassParser.Parse(parsingItem);

        string? type = null;
        if (parsingItem.Blocks[0].Lines.Count >= 2)
        {
            type = parsingItem.Blocks[0].Lines[^1].Text;
            parsingItem.Blocks[0].Lines[^1].Parsed = true;
        }

        string? name = null;
        if (parsingItem.Blocks[0].Lines.Count >= 3 && !parsingItem.Blocks[0].Lines[^2].Parsed)
        {
            name = parsingItem.Blocks[0].Lines[^2].Text;
            parsingItem.Blocks[0].Lines[^2].Parsed = true;
        }

        if (TryParseVaalGem(parsingItem, rarity, out var vaalGem) && vaalGem != null)
        {
            name = vaalGem.Name;
            type = vaalGem.Type;
        }

        return new ItemHeader()
        {
            Rarity = rarity,
            ItemClass = itemClass,
            Name = name,
            Type = type,
        };
    }

    private ApiItem? GetApiItem(ItemHeader header)
    {
        if (header.ItemClass == ItemClass.UncutSkillGem && apiItemProvider.IdDictionary.TryGetValue(apiInvariantItemProvider.UncutSkillGemId, out var uncutSkillGem))
        {
            return uncutSkillGem;
        }

        if (header.ItemClass == ItemClass.UncutSupportGem && apiItemProvider.IdDictionary.TryGetValue(apiInvariantItemProvider.UncutSupportGemId, out var uncutSupportGem))
        {
            return uncutSupportGem;
        }

        if (header.ItemClass == ItemClass.UncutSpiritGem && apiItemProvider.IdDictionary.TryGetValue(apiInvariantItemProvider.UncutSpiritGemId, out var uncutSpiritGem))
        {
            return uncutSpiritGem;
        }

        // Rares may have conflicting names, so we don't want to search any unique items that may have that name. Like "Ancient Orb" which can be used by abyss jewels.
        var name = header.Rarity is Rarity.Rare or Rarity.Magic ? null : header.Name;
        name = name != null ? GetLineWithoutSuperiorAffix(name) : null;
        var type = header.Type != null ? GetLineWithoutSuperiorAffix(header.Type) : null;

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

    private bool TryParseVaalGem(ParsingItem parsingItem, Rarity rarity, out ApiItem? vaalGem)
    {
        var canBeVaalGem = rarity == Rarity.Gem && parsingItem.Blocks.Count > 7;
        if (!canBeVaalGem || parsingItem.Blocks[5].Lines.Count <= 0)
        {
            vaalGem = null;
            return false;
        }

        apiItemProvider.NameAndTypeDictionary.TryGetValue(parsingItem.Blocks[5].Lines[0].Text, out var item);
        vaalGem = item?.FirstOrDefault();
        return true;
    }
}

