using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients.Exceptions;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Common;
using Sidekick.Common.Extensions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Clients;

public class PoeApiClient(
    ILogger<PoeTradeClient> logger,
    IHttpClientFactory httpClientFactory,
    ISettingsService settingsService) : IPoeApiClient
{
    private const string PoeApiUrl = "https://api.pathofexile.com/";

    private HttpClient CreateClient()
    {
        var httpClient = httpClientFactory.CreateClient(ClientNames.PoeClient);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        httpClient.BaseAddress = new Uri(PoeApiUrl);
        httpClient.Timeout = TimeSpan.FromHours(1);
        return httpClient;
    }

    public async Task<TReturn?> Fetch<TReturn>(string path)
    {
        using var httpClient = CreateClient();
        try
        {
            var response = await httpClient.GetAsync(path);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                await settingsService.Set(SettingKeys.BearerToken, null);
                await settingsService.Set(SettingKeys.BearerExpiration, null);
                throw new PoeApiException("Poe API: Unauthorized.");
            }

            var content = await response.Content.ReadAsStreamAsync();
            var result = await content.FromJsonToAsync<TReturn>(SerializationOptions.WithEnumConverterOptions);
            if (result != null)
            {
                return result;
            }
        }
        catch (TimeoutException e)
        {
            logger.LogError(e, "[Poe Api Client] The API is timed out due to too many requests.");
            return default;
        }
        catch (Exception e)
        {
            logger.LogError($"[Poe Api Client] Could not fetch {typeof(TReturn).Name} at {httpClient.BaseAddress + path}.", e);
            throw;
        }

        throw new PoeApiException("[Poe Api Client] Could not understand the API response.");
    }
}
