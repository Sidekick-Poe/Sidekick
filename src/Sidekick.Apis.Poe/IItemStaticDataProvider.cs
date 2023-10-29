using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe
{
    public interface IItemStaticDataProvider : IInitializableService
    {
        string? GetImage(string id);

        string? GetId(string? name, string? type);
    }
}
