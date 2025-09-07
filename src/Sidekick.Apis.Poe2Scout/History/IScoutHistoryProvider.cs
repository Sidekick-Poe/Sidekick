using Sidekick.Apis.Poe2Scout.History.Models;
using Sidekick.Common.Game.Items;
namespace Sidekick.Apis.Poe2Scout.History;

public interface IScoutHistoryProvider
{
    Task<ScoutHistory?> GetHistory(Rarity rarity, string? name, string? type);
}
