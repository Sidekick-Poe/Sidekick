using Sidekick.Apis.PoeNinja.Items.Models;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.PoeNinja.Items;

public interface INinjaItemProvider : IInitializableService
{
    NinjaPage? GetPage(string? invariantId);
}
