using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Static.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Static
{
    public class ItemStaticDataProvider : IItemStaticDataProvider
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeTradeClient poeTradeClient;

        public ItemStaticDataProvider(
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient)
        {
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
        }

        private Dictionary<string, string> ImageUrls { get; set; } = new();
        private Dictionary<string, string> Ids { get; set; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Medium;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            var result = await cacheProvider.GetOrSet(
                "ItemStaticDataProvider",
                () => poeTradeClient.Fetch<StaticItemCategory>("data/static"));

            ImageUrls.Clear();
            Ids.Clear();
            foreach (var category in result.Result)
            {
                foreach (var entry in category.Entries)
                {
                    if (entry.Id == null || entry.Image == null || entry.Text == null)
                    {
                        continue;
                    }

                    ImageUrls.Add(entry.Id, entry.Image);
                    if (!Ids.ContainsKey(entry.Text))
                    {
                        Ids.Add(entry.Text, entry.Id);
                    }
                }
            }
        }

        public string? GetImage(string id)
        {
            id = id switch
            {
                "exalt" => "exalted",
                _ => id
            };

            if (!string.IsNullOrEmpty(id) && ImageUrls.TryGetValue(id, out var result))
            {
                return result;
            }

            return null;
        }

        public string? GetId(Item item)
        {
            var text = item.Metadata.Name ?? item.Metadata.Type;
            if (text != null && Ids.TryGetValue(text, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
