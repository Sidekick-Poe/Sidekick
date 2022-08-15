using System.Threading.Tasks;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe
{
    public interface IItemParser
    {
        Item ParseItem(string itemText);
    }
}
