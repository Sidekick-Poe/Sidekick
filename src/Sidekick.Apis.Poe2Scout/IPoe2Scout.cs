using Sidekick.Apis.Poe2Scout.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe2Scout;

public interface IPoe2ScoutClient
{
    Task<Poe2ScoutPrice?> GetPriceInfo(string? englishName, string? englishType, Category category);

    Uri GetDetailsUri(Poe2ScoutPrice price);
}
