using Sidekick.Apis.PoeWiki.ApiModels;
using Sidekick.Apis.PoeWiki.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Apis.PoeWiki
{
    public interface IPoeWikiClient
    {
        public bool IsEnabled { get; }

        public Task<Map?> GetMap(Item item);

        public Task<List<string>?> GetOilsMetadataIdsFromEnchantment(ModifierLine modifierLine);

        public Task<List<ItemNameMetadataIdResult>?> GetMetadataIdsFromItemNames(List<string> itemNames);

        public void OpenUri(Map map);

        public void OpenUri(ItemDrop itemDrop);

        public void OpenUri(Boss boss);
    }
}
