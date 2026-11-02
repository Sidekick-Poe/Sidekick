using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Common.Extensions;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Common.Dialogs;
using Sidekick.Common.Settings;
using Sidekick.Game;
using Sidekick.Game.Providers.Languages;

namespace Sidekick.Apis.Poe.Trade.Authentication;

/// <summary>
/// Provides functionality to handle user authentication between the Sidekick application and the Path of Exile Account API.
/// Manages the authentication state, initializes authenticated HTTP requests,  and allows users to authenticate or reauthenticate their accounts.
/// </summary>
internal class AuthenticationService
(
    ISettingsService settingsService,
    ILogger<AuthenticationService> logger,
    BrowserDialogProvider browserDialogProvider,
    ICurrentGameLanguage currentGameLanguage
) : IAuthenticationService
{
    private const string CookieTokenName = "POETOKEN";

    private const string SettingsTokenName = "PoeTradeToken";

    public event Action? OnStateChanged;

    private Task? AuthenticateTask { get; set; }

    public async Task<AuthenticationState> GetCurrentState()
    {
        if (AuthenticateTask != null) return AuthenticationState.InProgress;

        var bearerToken = await settingsService.GetString(SettingsTokenName);
        if (bearerToken == null) return AuthenticationState.Unauthenticated;
        return !IsTokenExpired(bearerToken) ? AuthenticationState.Authenticated : AuthenticationState.Unauthenticated;
    }

    public async Task<DateTimeOffset?> GetExpiration()
    {
        var token = await settingsService.GetString(SettingsTokenName);
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return null;

        var jwtToken = handler.ReadJwtToken(token);
        return new DateTimeOffset(jwtToken.ValidTo, TimeSpan.Zero);
    }

    public async Task InitializeHttpRequest(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var cookieString = await browserDialogProvider.GetCookieString(TradeApiClient.ClientName, cancellationToken);
        request.AddOrUpdateCookie(cookieString);
    }

    public async Task Authenticate(bool reauthenticate = false, CancellationToken cancellationToken = default)
    {
        if (AuthenticateTask != null)
        {
            logger.LogInformation("[AuthenticationService] Authentication already in progress, waiting for it to finish.");
            await AuthenticateTask;
        }

        logger.LogInformation("[AuthenticationService] Starting authentication process.");
        var task = OpenBrowserWindow();
        AuthenticateTask = task;
        OnStateChanged?.Invoke();

        var result = await task;
        await browserDialogProvider.SaveCookies(TradeApiClient.ClientName, result, cancellationToken);

        await settingsService.Set(SettingsTokenName, result.Cookies.GetValueOrDefault(CookieTokenName));
        AuthenticateTask = null;
        OnStateChanged?.Invoke();
    }

    private async Task<BrowserDialogProvider.Result> OpenBrowserWindow()
    {
        var game = await settingsService.GetGame();
        var url = game == GameType.Poe1 ? currentGameLanguage.Language.PoeTradeBaseUrl : currentGameLanguage.Language.Poe2TradeBaseUrl;
        var builder = new UriBuilder(url);
        var authenticationLink = builder.ToString();
        return await browserDialogProvider.OpenBrowserWindow(new Uri(authenticationLink), ShouldComplete);

        bool ShouldComplete(BrowserDialogProvider.ShouldCompleteArgs args)
        {
            if (!args.Cookies.TryGetValue(CookieTokenName, out var token)) return false;

            return !IsTokenExpired(token);
        }
    }

    private bool IsTokenExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return true;

        var jwtToken = handler.ReadJwtToken(token);
        return DateTime.UtcNow >= jwtToken.ValidTo;
    }
}
