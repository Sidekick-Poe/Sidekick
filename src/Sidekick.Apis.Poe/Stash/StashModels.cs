using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Sidekick.Apis.Poe.Stash.Models
{
    public enum FrameType
    {
        Normal,
        Magic,
        Rare,
        Unique,
        Gem,
        Currency,
        DivinationCard,
        Quest,
        Prophecy,
        Foil,
        SupporterFoil
    }

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
        public string? name { get; set; }
        public string? type { get; set; }
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
        public required string league { get; set; }
        public required int ilvl { get; set; }
        public int? stackSize { get; set; }
        public int? maxStackSize { get; set; }
        public List<APIItemProperty>? properties { get; set; }
        public FrameType? frameType { get; set; }

        public string getFriendlyName()
        {
            if (name == "")
            {
                if (typeLine == "")
                {
                    return baseType;
                }
                return typeLine;
            }
            return name;
        }
    }

    public class APIItemProperty
    {
        public required string name { get; set; }
        public int? type { get; set; }
        //public List<List<String>>? values { get; set; }
    }

}
