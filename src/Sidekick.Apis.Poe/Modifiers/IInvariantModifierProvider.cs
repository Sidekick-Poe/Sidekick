using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Modifiers
{
    public interface IInvariantModifierProvider : IInitializableService
    {
        List<string> IncursionRoomModifierIds { get; }

        List<string> LogbookFactionModifierIds { get; }

        Task<List<ApiCategory>> GetList();
    }
}
