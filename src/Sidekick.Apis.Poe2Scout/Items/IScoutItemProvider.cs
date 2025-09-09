using Sidekick.Apis.Poe2Scout.Items.Models;
namespace Sidekick.Apis.Poe2Scout.Items;

public interface IScoutItemProvider
{
    Task<ScoutItem?> GetItem(string? name, string? type);
    Task<ScoutItem?> GetExaltedOrb() => GetItem("Exalted Orb", null);
    Task<ScoutItem?> GetChaosOrb() => GetItem("Chaos Orb", null);
    Task<ScoutItem?> GetDivineOrb() => GetItem("Divine Orb", null);
}
