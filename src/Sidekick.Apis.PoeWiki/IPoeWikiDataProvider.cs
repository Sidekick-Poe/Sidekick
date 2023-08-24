using System.Collections.Generic;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.PoeWiki
{
    public interface IPoeWikiDataProvider : IInitializableService
    {
        Dictionary<string, string> BlightOilNamesByMetadataIds { get; }
    }
}
