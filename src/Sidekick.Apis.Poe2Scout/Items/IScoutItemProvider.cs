using Sidekick.Apis.Poe2Scout.Items.Models;
namespace Sidekick.Apis.Poe2Scout.Items;

public interface IScoutItemProvider
{
    Task<ScoutItem?> GetItem(string? text);
    Task<ScoutItem?> GetExaltedOrb() => GetItem("Exalted Orb");
    Task<ScoutItem?> GetChaosOrb() => GetItem("Chaos Orb");
    Task<ScoutItem?> GetDivineOrb() => GetItem("Divine Orb");
}
