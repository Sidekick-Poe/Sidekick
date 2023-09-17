using Sidekick.Apis.PoeWiki.Api;
using Sidekick.Apis.PoeWiki.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.PoeWiki
{
    public interface IPoeWikiClient : IInitializableService
    {
        public bool IsEnabled { get; }

        Dictionary<string, string> BlightOilNamesByMetadataIds { get; }

        public Task<Map?> GetMap(Item item);

        public Task<List<string>?> GetOilsMetadataIdsFromEnchantment(ModifierLine modifierLine);

        public Task<List<ItemNameMetadataIdResult>?> GetMetadataIdsFromItemNames(List<string> itemNames);

        public void OpenUri(Map map);

        public void OpenUri(ItemDrop itemDrop);

        public void OpenUri(Boss boss);
    }
}
