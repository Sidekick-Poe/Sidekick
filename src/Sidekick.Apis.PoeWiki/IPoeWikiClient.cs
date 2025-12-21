using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeWiki.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.PoeWiki;

public interface IPoeWikiClient : IInitializableService
{
    Dictionary<string, string> BlightOilNamesByMetadataIds { get; }

    public Task<Map?> GetMap(string? mapType);

    public Task<List<string>> GetOilsMetadataIdsFromEnchantment(Stat stat);

    public void OpenUri(Map map);

    public void OpenUri(ItemDrop itemDrop);

    public void OpenUri(Boss boss);
}
