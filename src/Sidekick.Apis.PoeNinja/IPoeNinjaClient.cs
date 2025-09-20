using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Models;

namespace Sidekick.Apis.PoeNinja;

public interface IPoeNinjaClient
{
    Task<NinjaPrice?> GetPriceInfo(
        string? englishName,
        string? englishType,
        Category category,
        int? gemLevel = null,
        int? mapTier = null,
        bool? isRelic = false,
        int? numberOfLinks = null);

    Task<NinjaPrice?> GetClusterPrice(
        string englishGrantText,
        int passiveCount,
        int itemLevel);

    Task<Uri> GetDetailsUri(NinjaPrice price);
}
