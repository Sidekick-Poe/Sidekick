using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Sidekick.Apis.Poe.Authentication.Models;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform.Interprocess;

namespace Sidekick.Apis.Poe.Authentication
{
    internal class AuthenticationService : IAuthenticationService, IDisposable
    {
        private const string AUTHORIZATIONURL = "https://www.pathofexile.com/oauth/authorize";
        private const string REDIRECTURL = "https://sidekick-poe.github.io/oauth/poe";
        private const string CLIENTID = "sidekick";
        private const string SCOPES = "account:stashes";
        private const string TOKENURL = "https://www.pathofexile.com/oauth/token";

        private readonly ISettingsService settingsService;
        private readonly IBrowserProvider browserProvider;
        private readonly IInterprocessService interprocessService;
        private readonly HttpClient client;

        public event Action? OnAuthenticated;
        public event Action? OnStateChanged;

        public AuthenticationService(
            ISettingsService settingsService,
            IBrowserProvider browserProvider,
            IHttpClientFactory clientFactory,
            IInterprocessService interprocessService)
        {
            this.settingsService = settingsService;
            this.browserProvider = browserProvider;
            this.interprocessService = interprocessService;

            client = clientFactory.CreateClient();
            interprocessService.OnMessageReceived += InterprocessService_CustomProtocolCallback;
        }

        private string? State { get; set; }
        private string? Verifier { get; set; }
        private string? Challenge { get; set; }
        private TaskCompletionSource? AuthenticateTask { get; set; }

        public AuthenticationState CurrentState
        {
            get
            {
                if (AuthenticateTask != null && AuthenticateTask.Task.Status != TaskStatus.RanToCompletion)
                {
                    return AuthenticationState.InProgress;
                }

                var settings = settingsService.GetSettings();
                if (settings.Bearer_Expiration == null || string.IsNullOrEmpty(settings.Bearer_Token))
                {
                    return AuthenticationState.Unauthenticated;
                }

                if (DateTimeOffset.Now < settings.Bearer_Expiration)
                {
                    return AuthenticationState.Authenticated;
                }

                return AuthenticationState.Unauthenticated;
            }
        }

        public string? GetToken()
        {
            OnStateChanged?.Invoke();

            if (CurrentState != AuthenticationState.Authenticated)
            {
                return null;
            }

            var settings = settingsService.GetSettings();
            return settings.Bearer_Token;

        }

        public Task Authenticate(bool reauthenticate = false)
        {
            if (!reauthenticate && CurrentState == AuthenticationState.Authenticated)
            {
                return Task.CompletedTask;
            }

            if (CurrentState == AuthenticationState.InProgress)
            {
                return AuthenticateTask!.Task;
            }

            State = Guid.NewGuid().ToString();
            Verifier = GenerateCodeVerifier();
            Challenge = GenerateCodeChallenge(Verifier);

            var authenticationLink = $"{AUTHORIZATIONURL}?client_id={CLIENTID}&response_type=code&scope={SCOPES}&state={State}&redirect_uri={REDIRECTURL}&code_challenge={Challenge}&code_challenge_method=S256";
            browserProvider.OpenUri(new Uri(authenticationLink));

            AuthenticateTask = new();
            OnStateChanged?.Invoke();

            Task.Run(async () =>
            {
                await Task.Delay(30000);
                AuthenticateTask = null;
                OnStateChanged?.Invoke();
            });

            return AuthenticateTask.Task;
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
            if (!message.ToUpper().StartsWith("SIDEKICK://OAUTH/POE"))
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

        private async Task RequestAccessToken(string state, string code)
        {
            if (state != State)
            {
                CancelAuthenticate();
                return;
            }

            var requestContent = new StringContent(
                $"client_id={CLIENTID}&grant_type=authorization_code&code={code}&redirect_uri={REDIRECTURL}&scope={SCOPES}&code_verifier={Verifier}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            );
            var response = await client.PostAsync(TOKENURL, requestContent);
            var responseContent = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<Oauth2TokenResponse>(responseContent);

            if (result == null || result.access_token == null)
            {
                CancelAuthenticate();
                return;
            }

            await settingsService.Save(nameof(Settings.Bearer_Token), result.access_token);
            await settingsService.Save(nameof(Settings.Bearer_Expiration), DateTimeOffset.Now.AddSeconds(result.expires_in));

            if (AuthenticateTask != null)
            {
                AuthenticateTask.SetResult();
            }

            OnAuthenticated?.Invoke();
        }

        private static string GenerateCodeVerifier()
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);

            var codeVerifier = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
            return codeVerifier;
        }

        private static string GenerateCodeChallenge(string verifier)
        {
            string codeChallenge;
            using var sha256 = SHA256.Create();
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
            codeChallenge = Convert.ToBase64String(challengeBytes)
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
}
