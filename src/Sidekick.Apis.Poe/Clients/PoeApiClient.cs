using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Clients
{
    public class PoeApiClient : IPoeApiClient
    {
        private const string POEAPIURL = "https://api.pathofexile.com/";

        private readonly ILogger logger;
        private readonly ISettingsService settingsService;

        public PoeApiClient(
            ILogger<PoeTradeClient> logger,
            IHttpClientFactory httpClientFactory,
            ISettingsService settingsService)
        {
            this.logger = logger;
            this.settingsService = settingsService;

            HttpClient = httpClientFactory.CreateClient("PoeClient");
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
            HttpClient.BaseAddress = new Uri(POEAPIURL);
            HttpClient.Timeout = TimeSpan.FromHours(1);

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

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    await settingsService.Save("Bearer_Token", null);
                    await settingsService.Save("Bearer_Expiration", null);
                    throw new PoeApiException("Poe API: Unauthorized.");
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

            throw new PoeApiException("[Poe Api Client] Could not understand the API response.");
        }
    }
}
