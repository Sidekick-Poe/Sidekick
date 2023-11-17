using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Clients
{
    public class PoeApiClient : IPoeApiClient
    {
        private const string POEAPIURL = "https://api.pathofexile.com/";

        private readonly ILogger logger;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly IAuthenticationService authenticationService;
        private readonly ISettingsService settingsService;

        public PoeApiClient(
            ILogger<PoeTradeClient> logger,
            IGameLanguageProvider gameLanguageProvider,
            IHttpClientFactory httpClientFactory,
            IAuthenticationService authenticationService,
            ISettingsService settingsService)
        {
            this.logger = logger;
            this.gameLanguageProvider = gameLanguageProvider;
            this.authenticationService = authenticationService;
            this.settingsService = settingsService;

            HttpClient = httpClientFactory.CreateClient("PoeClient");
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
            HttpClient.BaseAddress = new Uri(POEAPIURL);

            Options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            Options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        public JsonSerializerOptions Options { get; }

        private HttpClient HttpClient { get; set; }

        public async Task<TReturn> Fetch<TReturn>(string path)
        {
            try
            {
                var response = await HttpClient.GetAsync(path);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await settingsService.Save("Bearer_Token", null);
                    await settingsService.Save("Bearer_Expiration", null);
                    throw new Exception("Poe API: Unauthorized.");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    await settingsService.Save("Bearer_Token", null);
                    await settingsService.Save("Bearer_Expiration", null);
                    throw new Exception("Poe API: Too Many Requests.");
                }

                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<TReturn>(content, Options);
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception e)
            {
                logger.LogError($"[Poe Api Client] Could not fetch {typeof(TReturn).Name} at {HttpClient.BaseAddress + path}.", e);
                throw;
            }

            throw new Exception("[Poe Api Client] Could not understand the API response.");
        }
    }
}
