using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidekick.Apis.Poe.Stash.Models
{
    public class APIStashList
    {
        public required List<APIStashTab> stashes { get; set; }
    }

    public class APIStashTabWrapper
    {
        public APIStashTab stash { get; set; }
    }

    public class APIStashTab
    {

        public string id { get; set; }
        public string? parent { get; set; }
        public required string name { get; set; }
        public required string type { get; set; }
        public int Index { get; set; }
        public List<APIStashTab>? children { get; set; }
        public List<APIStashItem>? items { get; set; }
        public bool parse { get; set; } = false;

    }

    public class APIStashItem
    {
        public required string id { get; set; }
        public required string name { get; set; }
        public required string typeLine { get; set; }
        public required string baseType { get; set; }
        public required string icon { get; set; }
        public int stackSize { get; set; }
        public int maxStackSize { get; set; }

        public string getFriendlyName()
        {
            if (name == "")
            {
                return typeLine;
            }
            return name;
        }
    }

}
