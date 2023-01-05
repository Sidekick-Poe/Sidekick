using Sidekick.Apis.PoeWiki.ApiModels;

namespace Sidekick.Apis.PoeWiki.Models
{
    public class ItemDrop
    {
        public ItemDrop(MapItemResult itemResult)
        {
            Name = itemResult.Name;
        }

        public string Name { get; set; }
    }
}
