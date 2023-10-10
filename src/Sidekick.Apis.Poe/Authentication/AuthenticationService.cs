using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.Results;
using Sidekick.Common.Browser;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.WebRequestMethods;

namespace Sidekick.Apis.Poe.Authentication
{
    internal class AuthenticationService : IAuthenticationService
    {
        private const string REDIRECTURL = "https://sidekick-poe.github.io/oauth/poe";
        private const string AUTHORIZATIONURL = "https://www.pathofexile.com/oauth/authorize";
        private const string TOKENURL = "https://www.pathofexile.com/oauth/token";
        private const string CLIENTID = "sidekick";
        private const string SCOPES = "account:stashes";

        private string _code { get; set; }
        private string _state { get; set; }
        private string _verifier { get; set; }
        private string _challenge { get; set; }
        private string _token { get; set; }
        private bool _isAuthenticating { get; set; }

        private ISettings _settings { get; set; }
        private ISettingsService _settingsService { get; set; }
        private IBrowserProvider _browser { get; set; }
        private HttpClient _client { get; set; }
       
        public AuthenticationService(
            ISettings settings,
            ISettingsService settingsService,
            IBrowserProvider browser,
            IHttpClientFactory clientFactory)
        {
            _settings = settings;
            _settingsService = settingsService;
            _browser = browser;
            _client = clientFactory.CreateClient();
            _isAuthenticating = false;
        }

        public Task Authenticate()
        {
            if (!_isAuthenticating)
            {
                _isAuthenticating = true;
                _state = Guid.NewGuid().ToString();
                _verifier = GenerateCodeVerifier();
                _challenge = GenerateCodeChallenge();
                _browser.OpenUri(new Uri(GenerateUserLink()));
            }
            return Task.CompletedTask;
        }

        public Task AuthenticationCallback(string code, string state)
        {
            if(_state == state) {
                _code = code;
                _token = RequestAccessToken().Result;
            } 
            return Task.CompletedTask;
        }

        public async Task<string> GetAccessToken()
        {
            if(_isAuthenticating)
            {
                return String.Empty;
            }

            if (_settings.Bearer_Expiration == null || String.IsNullOrEmpty(_settings.Bearer_Token))
            {
                await Authenticate();
                return String.Empty;
            }

            if(_settings.Bearer_Expiration?.AddMinutes(-1) < DateTime.Now)
            {
                await _settingsService.Save("Bearer_Token", null);
                await _settingsService.Save("Bearer_Expiration", null);

                await Authenticate();
                return String.Empty;
            }

            return _settings.Bearer_Token;
        }

        private async Task<string> RequestAccessToken()
        {
            var requestContent = new StringContent(
                $"client_id={CLIENTID}&grant_type=authorization_code&code={_code}&redirect_uri={REDIRECTURL}&scope={SCOPES}&code_verifier={_verifier}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            );

            var response = await _client.PostAsync(TOKENURL, requestContent);
            var responseContent = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<Oauth2TokenResponse>(responseContent);

            await _settingsService.Save("Bearer_Token", result.access_token);
            await _settingsService.Save("Bearer_Expiration", DateTime.Now.AddSeconds(result.expires_in));

            _isAuthenticating = false;

            return result.access_token;
        }

        private string GenerateCodeVerifier()
        {
            //Generate a random string for our code verifier
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);

            var codeVerifier = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
            return codeVerifier;
        }

        private string GenerateCodeChallenge()
        {
            //generate the code challenge based on the verifier
            string codeChallenge;
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_verifier));
                codeChallenge = Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }

            return codeChallenge;
           
        }
        private string GenerateUserLink()
        {
            return $"{AUTHORIZATIONURL}?client_id={CLIENTID}&response_type=code&scope={SCOPES}&state={_state}&redirect_uri={REDIRECTURL}&code_challenge={_challenge}&code_challenge_method=S256";
        }

        private class Oauth2TokenResponse
        {

            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string scope { get; set; }
            public string username { get; set; }
            public string sub { get; set; }
            public string refresh_token { get; set; }
        }

    }


}
