using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients.Exceptions;
using Sidekick.Apis.Poe.Clients.Models;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Clients;

public class PoeApiClient : IPoeApiClient
{
    private const string PoeApiUrl = "https://api.pathofexile.com/";

    private readonly ILogger logger;
    private readonly ISettingsService settingsService;

    public PoeApiClient(
        ILogger<PoeTradeClient> logger,
        IHttpClientFactory httpClientFactory,
        ISettingsService settingsService)
    {
        this.logger = logger;
        this.settingsService = settingsService;

        HttpClient = httpClientFactory.CreateClient(ClientNames.PoeClient);
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
        HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        HttpClient.BaseAddress = new Uri(PoeApiUrl);
        HttpClient.Timeout = TimeSpan.FromHours(1);

        Options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        Options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    private JsonSerializerOptions Options { get; }

    private HttpClient HttpClient { get; set; }

    public async Task<TReturn?> Fetch<TReturn>(string path)
    {
        try
        {
            var response = await HttpClient.GetAsync(path);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                await settingsService.Set(SettingKeys.BearerToken, null);
                await settingsService.Set(SettingKeys.BearerExpiration, null);
                throw new PoeApiException("Poe API: Unauthorized.");
            }

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TReturn>(content, Options);
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
            logger.LogError($"[Poe Api Client] Could not fetch {typeof(TReturn).Name} at {HttpClient.BaseAddress + path}.", e);
            throw;
        }

        throw new PoeApiException("[Poe Api Client] Could not understand the API response.");
    }
}
