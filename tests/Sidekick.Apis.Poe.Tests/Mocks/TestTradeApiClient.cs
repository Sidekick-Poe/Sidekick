using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Common.Exceptions;
namespace Sidekick.Apis.Poe.Tests.Mocks;

/// <summary>
/// Test-only Trade API client that always reads from local fallback data files
/// to avoid making HTTP calls during tests.
/// </summary>
public class TestTradeApiClient(ILogger<TestTradeApiClient> logger) : ITradeApiClient
{
    public async Task<FetchResult<TReturn>> FetchData<TReturn>(GameType game, IGameLanguage language, string path)
    {
        var dataFilePath = Path.GetFullPath(Path.Combine(GetProjectDirectory(), @"..\..\data", TradeApiClient.GetDataFileName(game, language, path)));
        logger.LogInformation("[TestTradeApiClient] Loading data from local file: {dataFilePath}", dataFilePath);

        if (!File.Exists(dataFilePath))
        {
            throw new SidekickException("[TestTradeApiClient] Local data file not found: " + dataFilePath);
        }

        await using var fileStream = File.OpenRead(dataFilePath);
        var result = await JsonSerializer.DeserializeAsync<FetchResult<TReturn>>(fileStream, TradeApiClient.JsonSerializerOptions);
        if (result == null || result.Result.Count == 0)
        {
            throw new SidekickException("[TestTradeApiClient] Failed to deserialize or empty result for file: " + dataFilePath);
        }

        return result;
    }

    public async Task<Stream> FetchData(GameType game, IGameLanguage language, string path)
    {
        var dataFilePath = Path.GetFullPath(Path.Combine(GetProjectDirectory(), @"..\..\data", TradeApiClient.GetDataFileName(game, language, path)));
        logger.LogInformation("[TestTradeApiClient] Loading stream from local file: {dataFilePath}", dataFilePath);

        if (!File.Exists(dataFilePath))
        {
            throw new SidekickException("[TestTradeApiClient] Local data file not found: " + dataFilePath);
        }

        // Return an open read-only stream
        return await Task.FromResult<Stream>(File.OpenRead(dataFilePath));
    }

    private static string GetProjectDirectory([CallerFilePath] string filePath = "")
    {
        var directory = Path.GetDirectoryName(filePath);
        while (directory != null && !Directory.EnumerateFiles(directory, "*.csproj").Any())
        {
            directory = Directory.GetParent(directory)?.FullName;
        }
        return directory ?? throw new FileNotFoundException("Could not find project directory.");
    }
}
