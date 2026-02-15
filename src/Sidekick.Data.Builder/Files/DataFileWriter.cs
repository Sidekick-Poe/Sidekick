using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Data.Options;

namespace Sidekick.Data.Files;

public class DataFileWriter(
    ILogger<DataFileWriter> logger,
    IOptions<DataOptions> options)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task Write(string game, string source, string fileName, object data)
    {
        if (string.IsNullOrWhiteSpace(options.Value.DataFolder))
        {
            throw new ArgumentException("Data folder cannot be null for writing.");
        }

        var path = Path.Combine(options.Value.DataFolder, game, source, fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(fs, data, JsonOptions);
        logger.LogInformation($"Saved {fileName}");
    }

    public async Task Write(string game, string source, string fileName, Stream stream)
    {
        if (string.IsNullOrWhiteSpace(options.Value.DataFolder))
        {
            throw new ArgumentException("Data folder cannot be null for writing.");
        }

        var path = Path.Combine(options.Value.DataFolder, game, source, fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fs);
        logger.LogInformation($"Saved {fileName}");
    }
}