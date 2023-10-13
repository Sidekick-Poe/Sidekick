using System.Net.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Clients
{
    public class PoeApiClient
    {
        private readonly ILogger logger;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly IAuthenticationService authenticationService;

        public PoeApiClient(
            ILogger<PoeTradeClient> logger,
            IGameLanguageProvider gameLanguageProvider,
            IHttpClientFactory httpClientFactory,
            IAuthenticationService authenticationService)
        {                                
            this.logger = logger;
            this.gameLanguageProvider = gameLanguageProvider;
            this.authenticationService = authenticationService;
            HttpClient = httpClientFactory.CreateClient("PoeClient");
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
            HttpClient.Timeout = TimeSpan.FromMinutes(360); // GGG API will rate limit us and we have to wait 5 minutes for the next request

            Options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            Options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        public JsonSerializerOptions Options { get; }

        public HttpClient HttpClient { get; set; }

        public void Authenticate()
        {
            authenticationService.Authenticate();
        }

        public void AuthenticationCallback(string code, string state)
        {
            authenticationService.AuthenticationCallback(code, state);
        }
    }
}
