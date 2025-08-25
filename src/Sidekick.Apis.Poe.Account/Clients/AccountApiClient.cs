using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Account.Clients;

public class AccountApiClient
(
    IHttpClientFactory httpClientFactory
) : IAccountApiClient
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
        var response = await httpClient.GetAsync(PoeApiUrl + path);
        var content = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<TReturn>(content, JsonSerializerOptions);
    }
}
