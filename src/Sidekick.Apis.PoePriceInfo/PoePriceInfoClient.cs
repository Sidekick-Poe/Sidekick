using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.PoePriceInfo.Api;
using Sidekick.Apis.PoePriceInfo.Models;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoePriceInfo;

public class PoePriceInfoClient(
    ISettingsService settingsService,
    ILogger<PoePriceInfoClient> logger,
    IHttpClientFactory httpClientFactory,
    ICurrentGameLanguage currentGameLanguage) : IPoePriceInfoClient
{
    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private HttpClient GetHttpClient()
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://www.poeprices.info/api");
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        client.Timeout = TimeSpan.FromSeconds(60);
        return client;
    }

    public async Task<PricePrediction?> GetPricePrediction(Item item)
    {
        if (item.Properties.Rarity != Rarity.Rare || !currentGameLanguage.IsEnglish())
        {
            return null;
        }

        try
        {
            var league = await settingsService.GetLeague();
            var encodedItem = Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Text.Text));
            using var client = GetHttpClient();
            var response = await client.GetAsync("?l=" + league + "&i=" + encodedItem);
            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<PriceInfoResult>(content, JsonSerializerOptions);

            if (result == null)
            {
                return null;
            }

            if (result is { Min: 0, Max: 0 })
            {
                return null;
            }

            return new PricePrediction()
            {
                ConfidenceScore = result.ConfidenceScore,
                Currency = result.Currency,
                Max = result.Max ?? 0,
                Min = result.Min ?? 0,
            };
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Error while trying to get price prediction from poeprices.info.");
        }

        return null;
    }
}
