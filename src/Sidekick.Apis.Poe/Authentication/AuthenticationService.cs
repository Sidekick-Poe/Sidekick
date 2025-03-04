using System.Security.Cryptography;
using System.Text;
using Sidekick.Apis.Poe.Authentication.Models;
using Sidekick.Common.Browser;
using Sidekick.Common.Extensions;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Authentication;

internal class AuthenticationService : IAuthenticationService, IDisposable
{
    private const string Authorizationurl = "https://www.pathofexile.com/oauth/authorize";
    private const string Redirecturl = "https://sidekick-poe.github.io/oauth/poe";
    private const string Clientid = "sidekick";
    private const string Scopes = "account:stashes";
    private const string Tokenurl = "https://www.pathofexile.com/oauth/token";

    private readonly ISettingsService settingsService;
    private readonly IBrowserProvider browserProvider;
    private readonly IInterprocessService interprocessService;
    private readonly IHttpClientFactory httpClientFactory;

    public event Action? OnStateChanged;

    public AuthenticationService(
        ISettingsService settingsService,
        IBrowserProvider browserProvider,
        IHttpClientFactory httpClientFactory,
        IInterprocessService interprocessService)
    {
        this.settingsService = settingsService;
        this.browserProvider = browserProvider;
        this.interprocessService = interprocessService;
        this.httpClientFactory = httpClientFactory;

        interprocessService.OnMessageReceived += InterprocessService_CustomProtocolCallback;
    }

    private string? State { get; set; }

    private string? Verifier { get; set; }

    private string? Challenge { get; set; }

    private TaskCompletionSource? AuthenticateTask { get; set; }

    public async Task<AuthenticationState> GetCurrentState()
    {
        if (AuthenticateTask != null && AuthenticateTask.Task.Status != TaskStatus.RanToCompletion)
        {
            return AuthenticationState.InProgress;
        }

        var bearerToken = await settingsService.GetString(SettingKeys.BearerToken);
        var bearerExpiration = await settingsService.GetDateTime(SettingKeys.BearerExpiration);
        if (bearerExpiration == null || string.IsNullOrEmpty(bearerToken))

        {
            return AuthenticationState.Unauthenticated;
        }

        if (DateTimeOffset.Now < bearerExpiration)
        {
            return AuthenticationState.Authenticated;
        }

        return AuthenticationState.Unauthenticated;
    }

    public async Task<string?> GetToken()
    {
        OnStateChanged?.Invoke();

        var currentState = await GetCurrentState();
        if (currentState != AuthenticationState.Authenticated)
        {
            return null;
        }

        return await settingsService.GetString(SettingKeys.BearerToken);
    }

    public async Task Authenticate(bool reauthenticate = false)
    {
        var currentState = await GetCurrentState();
        if (!reauthenticate && currentState == AuthenticationState.Authenticated)
        {
            return;
        }

        if (currentState == AuthenticationState.InProgress)
        {
            await AuthenticateTask!.Task;
            return;
        }

        State = Guid
                .NewGuid()
                .ToString();
        Verifier = GenerateCodeVerifier();
        Challenge = GenerateCodeChallenge(Verifier);

        var authenticationLink = $"{Authorizationurl}?client_id={Clientid}&response_type=code&scope={Scopes}&state={State}&redirect_uri={Redirecturl}&code_challenge={Challenge}&code_challenge_method=S256";
        browserProvider.OpenUri(new Uri(authenticationLink));

        AuthenticateTask = new();
        OnStateChanged?.Invoke();

        _ = Task.Run(
            async () =>
            {
                await Task.Delay(30000);
                AuthenticateTask = null;
                OnStateChanged?.Invoke();
            });

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
        if (!message
             .ToUpper()
             .StartsWith("SIDEKICK://OAUTH/POE"))
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

    private async Task RequestAccessToken(
        string state,
        string code)
    {
        if (state != State)
        {
            CancelAuthenticate();
            return;
        }

        using var client = httpClientFactory.CreateClient();
        var requestContent = new StringContent($"client_id={Clientid}&grant_type=authorization_code&code={code}&redirect_uri={Redirecturl}&scope={Scopes}&code_verifier={Verifier}", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await client.PostAsync(Tokenurl, requestContent);
        if (!response.IsSuccessStatusCode)
        {
            CancelAuthenticate();
            return;
        }

        var responseContent = await response.Content.ReadAsStreamAsync();
        var result = await responseContent.FromJsonToAsync<Oauth2TokenResponse>();

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

    private static string GenerateCodeVerifier()
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);

        var codeVerifier = Convert
                           .ToBase64String(bytes)
                           .TrimEnd('=')
                           .Replace('+', '-')
                           .Replace('/', '_');
        return codeVerifier;
    }

    private static string GenerateCodeChallenge(string verifier)
    {
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
        var codeChallenge = Convert
                        .ToBase64String(challengeBytes)
                        .TrimEnd('=')
                        .Replace('+', '-')
                        .Replace('/', '_');

        return codeChallenge;
    }

    public void Dispose()
    {
        interprocessService.OnMessageReceived -= InterprocessService_CustomProtocolCallback;
    }
}
