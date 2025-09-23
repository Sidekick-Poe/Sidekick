using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Items.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Items;

public interface IApiItemProvider : IInitializableService, IBaseItemProvider
{
    List<ItemApiInformation> UniqueItems { get; }
}
