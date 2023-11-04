using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.PoeNinja
{
    public interface IPoeNinjaClient
    {
        Task<NinjaPrice?> GetPriceInfo(
            string? englishName,
            string? englishType,
            Category category,
            int? gemLevel = null,
            int? mapTier = null,
            bool? isRelic = null,
            int? numberOfLinks = null);

        Task<NinjaPrice?> GetClusterPrice(
            string englishName,
            string englishType,
            List<string> englishGrantTexts,
            int passiveCount,
            int itemLevel);

        Uri GetDetailsUri(NinjaPrice price);
    }
}
