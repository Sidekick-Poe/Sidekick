using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Account.Clients.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Account.Clients;

public class PoeApiClient
(
    ILogger<PoeApiClient> logger,
    IHttpClientFactory httpClientFactory,
    ISettingsService settingsService
) : IPoeApiClient
{
    public const string ClientName = "PoeClient";
    private const string PoeApiUrl = "https://api.pathofexile.com/";

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task<TReturn?> Fetch<TReturn>(string path)
    {
        using var httpClient = httpClientFactory.CreateClient(ClientName);
        try
        {
            var response = await httpClient.GetAsync(PoeApiUrl + path);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                await settingsService.Set(SettingKeys.BearerToken, null);
                await settingsService.Set(SettingKeys.BearerExpiration, null);
                throw new PoeApiException("Poe API: Unauthorized.");
            }

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TReturn>(content, JsonSerializerOptions);
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
