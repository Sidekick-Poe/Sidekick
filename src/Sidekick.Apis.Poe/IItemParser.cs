using System.Threading.Tasks;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe
{
    public interface IItemParser
    {
        Task<Item> ParseItemAsync(string itemText);

        Item ParseItem(string itemText);
    }
}
