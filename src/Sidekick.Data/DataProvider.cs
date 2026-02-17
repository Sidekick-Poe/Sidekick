using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Apis.Poe.Items;
using Sidekick.Common;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;

namespace Sidekick.Data;

public class DataProvider
{
    private readonly IOptions<SidekickConfiguration> configuration;
    private readonly ILogger<DataProvider> logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private string DataDirectory { get; set; }

    public DataProvider(IOptions<SidekickConfiguration> configuration,
        ILogger<DataProvider> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
        DataDirectory = GetDataDirectory();

        return;

        string GetDataDirectory()
        {
#if DEBUG
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder)
            {
                var solutionDirectory = FindSolutionDirectory();
                if (!string.IsNullOrEmpty(solutionDirectory))
                {
                    return $"{solutionDirectory}/data";
                }
            }
#endif

            return Path.Combine(AppContext.BaseDirectory, "wwwroot/data/");
        }

        string? FindSolutionDirectory(string? startDirectory = null)
        {
            var dir = new DirectoryInfo(startDirectory ?? AppContext.BaseDirectory);

            while (dir != null)
            {
                var sln = dir.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly)
                    .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault();

                if (sln != null)
                    return sln.DirectoryName;

                dir = dir.Parent;
            }

            return null;
        }
    }

    public async Task Write(GameType game, string filePath, object data)
    {
        Directory.CreateDirectory(DataDirectory);

        var path = Path.Combine(DataDirectory, game.GetValueAttribute(), filePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(fs, data, JsonOptions);
        logger.LogInformation($"Saved {game.GetValueAttribute()}/{filePath}");
    }

    public async Task Write(GameType game, string filePath, Stream stream)
    {
        Directory.CreateDirectory(DataDirectory);

        var path = Path.Combine(DataDirectory, game.GetValueAttribute(), filePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fs);
        logger.LogInformation($"Saved {game.GetValueAttribute()}/{filePath}");
    }

    public async Task<TResult> Read<TResult>(GameType game, string filePath)
        where TResult : class
    {
        Directory.CreateDirectory(DataDirectory);

        var path = Path.Combine(DataDirectory, game.GetValueAttribute(), filePath);
        if (!File.Exists(path)) throw new SidekickException("The data file does not exist.");

        await using var fileStream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<TResult>(fileStream, JsonOptions)
               ?? throw new SidekickException("The data file could not be read successfully.");
    }

    public void DeleteAll()
    {
        if (configuration.Value.ApplicationType != SidekickApplicationType.DataBuilder)
        {
            throw new SidekickException("Can not delete all data except when running in CLI mode.");
        }

        if (!string.IsNullOrEmpty(DataDirectory))
        {
            Directory.Delete(DataDirectory, true);
            Directory.CreateDirectory(DataDirectory);
        }
    }
}