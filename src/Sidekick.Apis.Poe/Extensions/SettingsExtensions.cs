﻿using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Extensions;

public static class SettingsExtensions
{
    public static async Task<string?> GetLeague(this ISettingsService settingsService)
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        return leagueId.GetUrlSlugForLeague();
    }

    public static async Task<GameType> GetGame(this ISettingsService settingsService)
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        return leagueId.GetGameFromLeagueId();
    }

}
