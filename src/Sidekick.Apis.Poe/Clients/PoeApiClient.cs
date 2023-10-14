using System.Net.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Clients
{
    public class PoeApiClient: IPoeApiClient
    {
        private const string POEAPIURL = "https://api.pathofexile.com/";

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
            var name = typeof(TReturn).Name;

            try
            {
                var response = await HttpClient.GetAsync(path);

                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                    throw new Exception("Poe API: Unauthorized.");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests){
                    throw new Exception("Poe API: Too Many Requests.");
                }

                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<TReturn>(content, Options);
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception)
            {
                logger.LogInformation($"[Poe Api Client] Could not fetch {name} at {HttpClient.BaseAddress + path}.");
                throw;
            }

            throw new Exception("[Poe Api Client] Could not understand the API response.");
        }

    }
}
