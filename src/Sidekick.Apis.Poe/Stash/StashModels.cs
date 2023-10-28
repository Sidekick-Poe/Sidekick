using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public APIStashMetadata? metadata { get; set; }

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
        public List<APIItemSocket>? sockets { get; set; }

        public string getFriendlyName(bool includeMapTier = true)
        {
            if (includeMapTier) { 
                var mapTier = getMapTier();

                if (mapTier != null)
                {
                    return $"{typeLine} (Tier {mapTier})";
                }
            }
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

        public int? getMapTier()
        {
            if (properties != null)
            {
                var property = properties.FirstOrDefault(x => x.name.ToUpper() == "MAP TIER");

                if (property != null && property.values != null)
                {
                    var value = property.values[0];
                    if (value != null)
                    {
                        return value[0] == null ? 16 : Convert.ToInt32(value[0].ToString());
                    }

                }
            }
            return null;
        }

        public int? getLinkCount()
        {
            if(sockets != null)
            {
                return sockets.GroupBy(x => x.group).Max(x => x.Key);
            }
            return null;
        }

    }

    public class APIItemProperty
    {
        public required string name { get; set; }
        public int? type { get; set; }
        public List<List<object>>? values { get; set; }
    }

    public class APIStashMetadata
    {
        public int? items { get; set; }
        public APIStashMapItem? map { get; set; }
    }

    public class APIStashMapItem
    {
        public string? section { get; set; }
        public string? name { get; set; }
        public string? image { get; set; }
        public int? tier { get; set; }
        public int? series { get; set; }
    }

    public class APIItemSocket
    {
        public int group { get; set; }
        public string attr { get; set; }
        public string sColour { get; set; }
    }

}
