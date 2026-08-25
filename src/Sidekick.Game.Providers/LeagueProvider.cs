using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Extensions;
using Sidekick.Game.Languages;
using Sidekick.Game.Leagues;
namespace Sidekick.Game.Providers;

public class LeagueProvider(
    DataProvider dataProvider,
    IGameLanguageProvider languageProvider,
    ISettingsService settingsService) : IInitializableService
{
    private List<League>? Leagues { get; set; }

    public League Current
    {
        get => field ?? throw new InvalidOperationException("Current league is not set");
        private set;
    } = null!;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        leagueId = leagueId?.Split('.', 2).ElementAtOrDefault(1);
        var leagues = await GetLeagues();
        Current = leagues.FirstOrDefault(l => l.Game == game && l.Id == leagueId) ?? throw new InvalidOperationException("Current league is not set");
    }

    public async Task<List<League>> GetLeagues()
    {
        Leagues ??=
        [
            ..await dataProvider.Read<List<League>>(GameType.Poe2, GameDataType.Leagues, languageProvider.InvariantLanguage),
            ..await dataProvider.Read<List<League>>(GameType.Poe1, GameDataType.Leagues, languageProvider.InvariantLanguage),
        ];

        return Leagues;
    }

    public async Task Set(string value)
    {
        var game = value.GetGameFromLeagueId();
        var leagueId = value.Split('.', 2).ElementAtOrDefault(1);
        var leagues = await GetLeagues();
        Current = leagues.FirstOrDefault(l => l.Game == game && l.Id == leagueId) ?? throw new InvalidOperationException("Current league is not set");

        await settingsService.Set(SettingKeys.LeagueId, value);
    }
}
