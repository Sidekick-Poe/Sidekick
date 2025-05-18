using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Sidekick.Apis.Poe.Clients.Authentication.Models;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Clients.Authentication;

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

        var authenticationLink = $"{AuthenticationConfig.AuthorizationUrl}?client_id={AuthenticationConfig.ClientId}&response_type=code&scope={AuthenticationConfig.Scopes}&state={State}&redirect_uri={AuthenticationConfig.RedirectUrl}&code_challenge={PkcePair.Challenge}&code_challenge_method=S256";
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

        using var client = httpClientFactory.CreateClient(ClientNames.PoeClient);
        using var requestContent = new StringContent($"client_id={AuthenticationConfig.ClientId}&grant_type=authorization_code&code={code}&redirect_uri={AuthenticationConfig.RedirectUrl}&scope={AuthenticationConfig.Scopes}&code_verifier={PkcePair?.Verifier}", Encoding.UTF8, "application/x-www-form-urlencoded");
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
