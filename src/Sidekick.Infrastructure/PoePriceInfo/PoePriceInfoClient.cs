using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Domain.Settings;
using Sidekick.Infrastructure.PoePriceInfo.Models;

namespace Sidekick.Infrastructure.PoePriceInfo
{
    public class PoePriceInfoClient : IPoePriceInfoClient
    {
        private const string PoePricesBaseUrl = "https://www.poeprices.info/api";
        private readonly ILogger logger;
        private readonly ISidekickSettings settings;
        private readonly HttpClient client;

        public PoePriceInfoClient(ILogger<PoePriceInfoClient> logger,
            IHttpClientFactory httpClientFactory,
            ISidekickSettings settings)
        {
            this.logger = logger;
            this.settings = settings;
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(PoePricesBaseUrl);
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
            Options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private JsonSerializerOptions Options { get; }

        public async Task<PriceInfoResult> GetItemPricePrediction(string itemText)
        {
            var encodedItem = Convert.ToBase64String(Encoding.UTF8.GetBytes(itemText));

            try
            {
                var response = await client.GetAsync("?l=" + settings.LeagueId + "&i=" + encodedItem);
                var content = await response.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<PriceInfoResult>(content, Options);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception thrown while getting price prediction from poeprices.info. " + ex.Message);
            }

            return null;
        }
    }
}