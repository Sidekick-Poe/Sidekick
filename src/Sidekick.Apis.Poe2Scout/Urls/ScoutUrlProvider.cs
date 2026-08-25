using Sidekick.Game;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;
namespace Sidekick.Apis.Poe2Scout.Urls;

public class ScoutUrlProvider(LeagueProvider leagueProvider)
{
    public Uri? GetUri(Item item)
    {
        var realm = GetRealm(item);

        var scoutItem = item.Definition.ScoutItems?.FirstOrDefault();
        if (scoutItem == null) return null;

        var type = scoutItem.IsCurrency ? "currencies" : "uniques";

        return new Uri($"https://poe2scout.com/{realm}/{leagueProvider.Current.ScoutValue}/economy/{type}/{scoutItem.CategoryApiId}");
    }

    private string GetRealm(Item item)
    {
        return item.Game switch
        {
            GameType.Poe2 => "poe2",
            _ => "pc",
        };
    }
}
