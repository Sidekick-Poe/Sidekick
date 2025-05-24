using System.Net.Http.Headers;
using System.Text.Json;
using Sidekick.Apis.Poe.Account.Authentication.Models;
using Sidekick.Apis.Poe.Account.Clients;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Account.Authentication;

/// <summary>
/// Provides functionality to handle user authentication between the Sidekick application and the Path of Exile Account API.
/// Manages the authentication state, initializes authenticated HTTP requests,  and allows users to authenticate or reauthenticate their accounts.
/// </summary>
internal class AuthenticationService : IAuthenticationService, IDisposable
{
    private readonly ISettingsService settingsService;
    private readonly IBrowserProvider browserProvider;
    private readonly IInterprocessService interprocessService;
    private readonly IHttpClientFactory httpClientFactory;

    public event Action? OnStateChanged;

    public AuthenticationService(ISettingsService settingsService, IBrowserProvider browserProvider, IHttpClientFactory httpClientFactory, IInterprocessService interprocessService)
    {
        this.settingsService = settingsService;
        this.browserProvider = browserProvider;
        this.interprocessService = interprocessService;
        this.httpClientFactory = httpClientFactory;

        interprocessService.OnMessageReceived += InterprocessService_CustomProtocolCallback;
    }

    private string? State { get; set; }

    private PkceHelper.PkcePair? PkcePair { get; set; }

    private TaskCompletionSource? AuthenticateTask { get; set; }

    public async Task<AuthenticationState> GetCurrentState()
    {
        if (AuthenticateTask != null && AuthenticateTask.Task.Status != TaskStatus.RanToCompletion) return AuthenticationState.InProgress;

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
        await interprocessService.Install();

        var currentState = await GetCurrentState();
        if (!reauthenticate && currentState == AuthenticationState.Authenticated) return;

        if (currentState == AuthenticationState.InProgress)
        {
            await AuthenticateTask!.Task;
            return;
        }

        State = Guid.NewGuid().ToString();
        PkcePair = PkceHelper.GeneratePkcePair();

        var builder = new UriBuilder(AuthenticationConfig.AuthorizationUrl);
        var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
        query["client_id"] = AuthenticationConfig.ClientId;
        query["response_type"] = "code";
        query["scope"] = AuthenticationConfig.Scopes;
        query["state"] = State;
        query["redirect_uri"] = AuthenticationConfig.RedirectUrl;
        query["code_challenge"] = PkcePair.Challenge;
        query["code_challenge_method"] = "S256";
        builder.Query = query.ToString();
        var authenticationLink = builder.ToString();
        browserProvider.OpenUri(new Uri(authenticationLink));

        AuthenticateTask = new();
        OnStateChanged?.Invoke();

        _ = Task.Run(async () =>
                     {
                         await Task.Delay(AuthenticationConfig.AuthenticationTimeoutMs, cancellationToken);
                         AuthenticateTask?.SetCanceled(cancellationToken);
                         AuthenticateTask = null;
                         OnStateChanged?.Invoke();
                     },
                     cancellationToken);

        await AuthenticateTask.Task;
    }

    private void CancelAuthenticate()
    {
        if (AuthenticateTask != null)
        {
            AuthenticateTask.SetResult();
            AuthenticateTask = null;
        }

        OnStateChanged?.Invoke();
    }

    private void InterprocessService_CustomProtocolCallback(string message)
    {
        if (!message.ToUpper().StartsWith(AuthenticationConfig.ProtocolPrefix))
        {
            return;
        }

        var queryDictionary = System.Web.HttpUtility.ParseQueryString(new Uri(message).Query);
        var state = queryDictionary["state"];
        var code = queryDictionary["code"];

        if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(code))
        {
            CancelAuthenticate();
            return;
        }

        _ = RequestAccessToken(state, code);
    }

    private async Task RequestAccessToken(string state, string code, CancellationToken cancellationToken = default)
    {
        if (state != State)
        {
            CancelAuthenticate();
            return;
        }

        using var requestContent = new FormUrlEncodedContent([
            new("client_id", AuthenticationConfig.ClientId),
            new("grant_type", "authorization_code"),
            new("code", code),
            new("redirect_uri", AuthenticationConfig.RedirectUrl),
            new("scope", AuthenticationConfig.Scopes),
            new("code_verifier", PkcePair?.Verifier ?? string.Empty),
        ]);
        using var client = httpClientFactory.CreateClient(AccountApiClient.ClientName);
        var response = await client.PostAsync(AuthenticationConfig.TokenUrl, requestContent, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            CancelAuthenticate();
            return;
        }

        var result = JsonSerializer.Deserialize<Oauth2TokenResponse>(responseContent);
        if (result == null || result.access_token == null)
        {
            CancelAuthenticate();
            return;
        }

        await settingsService.Set(SettingKeys.BearerToken, result.access_token);
        await settingsService.Set(SettingKeys.BearerExpiration, DateTimeOffset.Now.AddSeconds(result.expires_in));

        if (AuthenticateTask != null)
        {
            AuthenticateTask.SetResult();
        }

        OnStateChanged?.Invoke();
    }

    public void Dispose()
    {
        interprocessService.OnMessageReceived -= InterprocessService_CustomProtocolCallback;
    }
}
