using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Initialization;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Data;

public record DataFile(string Path, string Name, long Size, DateTimeOffset LastModified);

public class DataProvider : IInitializableService
{
    private readonly IOptions<SidekickConfiguration> configuration;
    private readonly ILogger<DataProvider> logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public string DataDirectory { get; }

    public int Priority => -100;

    public DataProvider(IOptions<SidekickConfiguration> configuration, ILogger<DataProvider> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
        DataDirectory = GetDataDirectory();

        return;

        string GetDataDirectory()
        {
            if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
            {
                var solutionDirectory = FindSolutionDirectory();
                if (!string.IsNullOrEmpty(solutionDirectory))
                {
                    return $"{solutionDirectory}/data";
                }
            }

            return SidekickPaths.GetDataFilePath("SidekickData");
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

    public Task Initialize()
    {
        if (configuration.Value.ApplicationType == SidekickApplicationType.DataBuilder || configuration.Value.ApplicationType == SidekickApplicationType.Test)
        {
            return Task.CompletedTask;
        }

        var sourceDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot/data");
        var targetDirectory = DataDirectory;

        if (!Directory.Exists(sourceDirectory))
        {
            logger.LogWarning("Source directory for data files not found at {sourceDirectory}. Skipping data copy.", sourceDirectory);
            return Task.CompletedTask;
        }

        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        CopyDirectory(sourceDirectory, targetDirectory);

        return Task.CompletedTask;

        void CopyDirectory(string source, string target)
        {
            foreach (var sourceFile in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(source, sourceFile);
                var targetFile = Path.Combine(target, relativePath);

                var sourceInfo = new FileInfo(sourceFile);
                var targetInfo = new FileInfo(targetFile);

                if (!targetInfo.Exists || sourceInfo.LastWriteTimeUtc > targetInfo.LastWriteTimeUtc)
                {
                    var targetFileInfo = new FileInfo(targetFile);
                    if (targetFileInfo.Directory != null && !targetFileInfo.Directory.Exists)
                    {
                        targetFileInfo.Directory.Create();
                    }

                    File.Copy(sourceFile, targetFile, true);
                    logger.LogInformation($"Copied {relativePath} to user data folder.");
                }
            }
        }
    }

    public async Task Write(string filePath, object data)
    {
        Directory.CreateDirectory(DataDirectory);

        var path = Path.Combine(DataDirectory, filePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(fs, data, JsonOptions);
        logger.LogInformation($"Saved {path}");
    }

    public async Task Write(string filePath, Stream stream)
    {
        Directory.CreateDirectory(DataDirectory);

        var path = Path.Combine(DataDirectory, filePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fs);
        logger.LogInformation($"Saved {path}");
    }

    public Task Write(GameType game, DataType type, IGameLanguage language, object data)
    {
        return Write(GetFilePath(game, type, language.Code), data);
    }

    public Task Write(GameType game, DataType type, IGameLanguage language, Stream stream)
    {
        return Write(GetFilePath(game, type, language.Code), stream);
    }

    public Task Write(GameType game, DataType type, object data)
    {
        return Write(GetFilePath(game, type, "invariant"), data);
    }

    public Task Write(GameType game, DataType type, Stream stream)
    {
        return Write(GetFilePath(game, type, "invariant"), stream);
    }

    public async Task<TResult> Read<TResult>(string filePath)
        where TResult : class
    {
        Directory.CreateDirectory(DataDirectory);

        var path = Path.Combine(DataDirectory, filePath);
        if (!File.Exists(path)) throw new SidekickException($"The data file does not exist. {filePath}");

        await using var fileStream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<TResult>(fileStream, JsonOptions)
               ?? throw new SidekickException("The data file could not be read successfully.");
    }

    public Task<TResult> Read<TResult>(GameType game, DataType type, IGameLanguage language)
        where TResult : class
    {
        return Read<TResult>(GetFilePath(game, type, language.Code));
    }

    public Task<TResult> Read<TResult>(GameType game, DataType type)
        where TResult : class
    {
        return Read<TResult>(GetFilePath(game, type, "invariant"));
    }

    private string GetFilePath(GameType game, DataType type, string languageCode)
    {
        return game.GetValueAttribute() + "/" + string.Format(type.GetValueAttribute(), languageCode);
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

    public List<DataFile> GetFiles()
    {
        var files = new List<DataFile>();
        if (!Directory.Exists(DataDirectory))
        {
            return files;
        }

        var directoryInfo = new DirectoryInfo(DataDirectory);
        foreach (var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            files.Add(new DataFile(
                      Path: file.FullName,
                      Name: Path.GetRelativePath(DataDirectory, file.FullName).Replace('\\', '/'),
                      Size: file.Length,
                      LastModified: file.LastWriteTimeUtc
                      ));
        }

        return files;
    }
}
