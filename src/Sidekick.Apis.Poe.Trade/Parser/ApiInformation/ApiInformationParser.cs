using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.ApiItems;
using Sidekick.Common.Exceptions;
namespace Sidekick.Apis.Poe.Trade.Parser.ApiInformation;

public class ApiInformationParser(IApiItemProvider apiItemProvider) : IApiInformationParser
{
    public void Parse(Item item)
    {
        item.ApiInformation = GetItemInformation(item.Properties.Rarity, item.Name, item.Type)
                              ?? throw new UnparsableException(item.Text.Text);

        ParseVaalGem(item);
    }

    private ItemApiInformation? GetItemInformation(Rarity rarity, string? name, string? type)
    {
        // Rares may have conflicting names, so we don't want to search any unique items that may have that name. Like "Ancient Orb" which can be used by abyss jewels.
        name = rarity is Rarity.Rare or Rarity.Magic ? null : name;

        ItemApiInformation? result = null;

        result ??= string.IsNullOrEmpty(name) ? null :
            apiItemProvider.NamePatterns.Where(pattern => pattern.Regex.IsMatch(name))
                .Select(x => x.Item)
                .OrderByDescending(x => x.Name == name ? 1 : 0)
                .ThenByDescending(x => x.Type == type ? 1 : 0)
                .FirstOrDefault();

        result ??= string.IsNullOrEmpty(type) ? null :
            apiItemProvider.TextPatterns.Where(pattern => pattern.Regex.IsMatch(type))
                .Select(x => x.Item)
                .FirstOrDefault();

        result ??= string.IsNullOrEmpty(type) ? null :
            apiItemProvider.TypePatterns.Where(pattern => pattern.Regex.IsMatch(type))
                .Select(x => x.Item)
                .FirstOrDefault();

        return result;
    }

    private void ParseVaalGem(Item item)
    {
        var canBeVaalGem = item.Properties.ItemClass == ItemClass.ActiveGem && item.Text.Blocks.Count > 7;
        if (!canBeVaalGem || item.Text.Blocks[5].Lines.Count <= 0) return;

        var vaalGem = GetItemInformation(Rarity.Gem, null, item.Text.Blocks[5].Lines[0].Text);
        if (vaalGem != null)
        {
            item.ApiInformation = vaalGem;
        }
    }
}
