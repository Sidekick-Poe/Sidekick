using System.Threading.Tasks;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe
{
    public interface IItemParser
    {
        Task Initialize();

        Item ParseItem(string itemText);
    }
}
