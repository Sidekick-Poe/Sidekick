using System.Threading.Tasks;
using Sidekick.Apis.PoeWiki.ApiModels;
using Sidekick.Apis.PoeWiki.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.PoeWiki
{
    public interface IPoeWikiClient
    {
        public Task<Map> GetMap(Item item);
    }
}
