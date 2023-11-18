namespace Sidekick.Apis.Poe.Stash.Models
{
    public class APIStashItem
    {
        public required string id { get; set; }
        public required string name { get; set; }
        public required string? typeLine { get; set; }
        public required string? baseType { get; set; }
        public required string? icon { get; set; }
        public required string league { get; set; }
        public required int ilvl { get; set; }
        public int? stackSize { get; set; }
        public int? maxStackSize { get; set; }
        public List<APIItemProperty>? properties { get; set; }
        public FrameType? frameType { get; set; }
        public List<APIItemSocket>? sockets { get; set; }

        public string getFriendlyName()
        {
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            if (!string.IsNullOrEmpty(typeLine))
            {
                return typeLine;
            }

            return baseType ?? "";
        }

        public int? getMapTier()
        {
            return getPropertyValue("MAP TIER", 16);
        }

        public int? getGemLevel()
        {
            return getPropertyValue("LEVEL", 16);
        }

        public int? getLinkCount()
        {
            if (sockets != null)
            {
                return sockets.GroupBy(x => x.group).Max(x => x.Key);
            }
            return null;
        }

        private int? getPropertyValue(string name, int defaultValue = 0)
        {
            if (properties != null)
            {
                var property = properties.FirstOrDefault(x => x.name.ToUpper() == name.ToUpper());

                if (property != null && property.values != null)
                {
                    var value = property.values[0];
                    if (value != null)
                    {
                        return value[0] == null ? defaultValue : Convert.ToInt32(value[0].ToString());
                    }
                }
            }
            return null;
        }
    }
}
