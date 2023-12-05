using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Clients
{
    public class PoeTradeClient : IPoeTradeClient
    {
        private readonly ILogger logger;
        private readonly IGameLanguageProvider gameLanguageProvider;

        public PoeTradeClient(
            ILogger<PoeTradeClient> logger,
            IGameLanguageProvider gameLanguageProvider,
            IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.gameLanguageProvider = gameLanguageProvider;
            HttpClient = httpClientFactory.CreateClient(ClientNames.TRADECLIENT);
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");

            Options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            Options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        public JsonSerializerOptions Options { get; }

        public HttpClient HttpClient { get; set; }

        public async Task<FetchResult<TReturn>> Fetch<TReturn>(string path, bool useDefaultLanguage = false)
        {
            var name = typeof(TReturn).Name;

            var language = gameLanguageProvider.Language;

            if (useDefaultLanguage || language == null)
            {
                language = gameLanguageProvider.Get("en");
            }

            try
            {
                var response = await HttpClient.GetAsync(language?.PoeTradeApiBaseUrl + path);
                var content = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<FetchResult<TReturn>>(content, Options);
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception)
            {
                logger.LogInformation($"[Trade Client] Could not fetch {name} at {language?.PoeTradeApiBaseUrl + path}.");
                throw;
            }

            throw new Exception("[Trade Client] Could not understand the API response.");
        }
    }
}
