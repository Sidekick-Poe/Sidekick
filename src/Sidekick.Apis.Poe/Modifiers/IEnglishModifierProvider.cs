using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Modifiers
{
    public interface IEnglishModifierProvider : IInitializableService
    {
        List<string> IncursionRooms { get; }

        List<string> LogbookFactions { get; }

        Task<List<ApiCategory>> GetList();
    }
}
