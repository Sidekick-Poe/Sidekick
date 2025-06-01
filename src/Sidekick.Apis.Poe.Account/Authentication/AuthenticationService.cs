using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Account.Authentication.Models;
using Sidekick.Apis.Poe.Account.Clients;
using Sidekick.Common.Browser;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Account.Authentication;

/// <summary>
/// Provides functionality to handle user authentication between the Sidekick application and the Path of Exile Account API.
/// Manages the authentication state, initializes authenticated HTTP requests,  and allows users to authenticate or reauthenticate their accounts.
/// </summary>
internal class AuthenticationService
(
    ISettingsService settingsService,
    IHttpClientFactory httpClientFactory,
    ILogger<AuthenticationService> logger,
    IBrowserWindowProvider browserWindowProvider
) : IAuthenticationService
{
    public event Action? OnStateChanged;

    private Task? AuthenticateTask { get; set; }

    public async Task<AuthenticationState> GetCurrentState()
    {
        if (AuthenticateTask != null) return AuthenticationState.InProgress;

        var bearerToken = await settingsService.GetString(SettingKeys.BearerToken);
        var bearerExpiration = await settingsService.GetDateTime(SettingKeys.BearerExpiration);
        if (bearerExpiration == null || string.IsNullOrEmpty(bearerToken)) return AuthenticationState.Unauthenticated;
        return DateTimeOffset.Now < bearerExpiration ? AuthenticationState.Authenticated : AuthenticationState.Unauthenticated;
    }

    private async Task<string?> GetToken()
    {
        OnStateChanged?.Invoke();

        var currentState = await GetCurrentState();
        if (currentState != AuthenticationState.Authenticated) return null;

        return await settingsService.GetString(SettingKeys.BearerToken);
    }

    public async Task InitializeHttpRequest(HttpRequestMessage request)
    {
        var token = await GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task Authenticate(bool reauthenticate = false, CancellationToken cancellationToken = default)
    {
        if (AuthenticateTask != null)
        {
            logger.LogInformation("[AuthenticationService] Authentication already in progress, waiting for it to finish.");
            await AuthenticateTask;
        }

        var state = Guid.NewGuid().ToString();
        var pkcePair = PkceHelper.GeneratePkcePair();

        logger.LogInformation("[AuthenticationService] Starting authentication process.");
        var task = OpenBrowserWindow(state, pkcePair);
        AuthenticateTask = task;
        OnStateChanged?.Invoke();

        var result = await task;
        await browserWindowProvider.SaveCookies(AccountApiClient.ClientName, result, cancellationToken);

        if (!result.Success)
        {
            logger.LogInformation("[AuthenticationService] Result was not successful, cancelling authentication.");
            AuthenticateTask = null;
            OnStateChanged?.Invoke();
            return;
        }

        if (result.Uri == null)
        {
            logger.LogInformation("[AuthenticationService] Result URI was null.");
            AuthenticateTask = null;
            OnStateChanged?.Invoke();
            return;
        }

        var queryDictionary = System.Web.HttpUtility.ParseQueryString(result.Uri.Query);
        var resultState = queryDictionary["state"];
        var resultCode = queryDictionary["code"];

        if (string.IsNullOrEmpty(resultState) || state != resultState)
        {
            logger.LogInformation("[AuthenticationService] The state was invalid.");
            AuthenticateTask = null;
            OnStateChanged?.Invoke();
            return;
        }

        if (string.IsNullOrEmpty(resultCode))
        {
            logger.LogInformation("[AuthenticationService] The code was invalid.");
            AuthenticateTask = null;
            OnStateChanged?.Invoke();
            return;
        }

        var token = await RequestAccessToken(resultCode, pkcePair.Verifier, cancellationToken);
        if (token == null)
        {
            logger.LogInformation("[AuthenticationService] The token was invalid.");
            AuthenticateTask = null;
            OnStateChanged?.Invoke();
            return;
        }

        await settingsService.Set(SettingKeys.BearerToken, token.access_token);
        await settingsService.Set(SettingKeys.BearerExpiration, DateTimeOffset.Now.AddSeconds(token.expires_in));
        AuthenticateTask = null;
        OnStateChanged?.Invoke();
    }

    private Task<BrowserResult> OpenBrowserWindow(string state, PkceHelper.PkcePair pkcePair)
    {
        var builder = new UriBuilder(AuthenticationConfig.AuthorizationUrl);
        var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
        query["client_id"] = AuthenticationConfig.ClientId;
        query["response_type"] = "code";
        query["scope"] = AuthenticationConfig.Scopes;
        query["state"] = state;
        query["redirect_uri"] = AuthenticationConfig.RedirectUrl;
        query["code_challenge"] = pkcePair.Challenge;
        query["code_challenge_method"] = "S256";
        builder.Query = query.ToString();
        var authenticationLink = builder.ToString();
        var task = browserWindowProvider.OpenBrowserWindow(new BrowserRequest()
        {
            Uri = new Uri(authenticationLink),
            ShouldComplete = (options) => options.Uri?.ToString().StartsWith(AuthenticationConfig.RedirectUrl) ?? false,
        });
        return task;
    }

    private async Task<Oauth2TokenResponse?> RequestAccessToken(string code, string verifier, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[AuthenticationService] The authentication was successful, requesting access token.");

        using var requestContent = new FormUrlEncodedContent([
            new("client_id", AuthenticationConfig.ClientId),
            new("grant_type", "authorization_code"),
            new("code", code),
            new("redirect_uri", AuthenticationConfig.RedirectUrl),
            new("scope", AuthenticationConfig.Scopes),
            new("code_verifier", verifier),
        ]);
        using var client = httpClientFactory.CreateClient(AccountApiClient.ClientName);
        var response = await client.PostAsync(AuthenticationConfig.TokenUrl, requestContent, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogInformation("[AuthenticationService] The token response is invalid.");
            return null;
        }

        var result = JsonSerializer.Deserialize<Oauth2TokenResponse>(responseContent);
        if (result == null || result.access_token == null)
        {
            logger.LogInformation("[AuthenticationService] The token response is invalid.");
            return null;
        }

        logger.LogInformation("[AuthenticationService] The access token call was successful.");
        return result;
    }
}
