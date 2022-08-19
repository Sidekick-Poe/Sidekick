using System.Threading.Tasks;
using Sidekick.Apis.PoeWiki.ApiModels;
using Sidekick.Apis.PoeWiki.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.PoeWiki
{
    public interface IPoeWikiClient
    {
        public bool IsEnabled { get; }
        public Task<Map> GetMap(Item item);
        public Task<WikiPage> GetWikiPage(int pageId);
        public void OpenUri(Map map);
        public void OpenUri(ItemDrop itemDrop);
        public void OpenUri(Boss boss);
    }
}
