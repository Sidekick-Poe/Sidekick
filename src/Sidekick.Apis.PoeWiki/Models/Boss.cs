using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sidekick.Apis.PoeWiki.ApiModels;

namespace Sidekick.Apis.PoeWiki.Models
{
    public class Boss
    {
        public Boss(BossResult boss)
        {
            Id = boss.MetadataId;
            Name = boss.Name;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}
