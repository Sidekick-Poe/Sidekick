using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sidekick.Apis.PoeWiki.ApiModels;

namespace Sidekick.Apis.PoeWiki.Models
{
    public class Map
    {
        public Map(MapResult map, List<BossResult> bosses, List<ItemResult> items)
        {
            Id = map.AreaId;
            Name = map.Name;
            Bosses = bosses.Select(x => new Boss(x)).ToList();
            Drops = items.Select(x => new ItemDrop(x)).ToList();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public List<Boss> Bosses { get; set; }
        public List<ItemDrop> Drops { get; set; }
    }
}
