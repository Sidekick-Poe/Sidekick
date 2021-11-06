using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Apis.Poe.Modifiers.Models;

namespace Sidekick.Apis.Poe.Modifiers
{
    public interface IEnglishModifierProvider
    {
        Task Initialize();
        Task<List<ApiCategory>> GetList();
        List<string> IncursionRooms { get; }
        List<string> LogbookFactions { get; }
    }
}
