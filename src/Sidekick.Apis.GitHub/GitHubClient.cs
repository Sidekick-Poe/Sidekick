using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.GitHub.Api;
using Sidekick.Apis.GitHub.Models;
using Sidekick.Common;
using Sidekick.Common.Cache;

namespace Sidekick.Apis.GitHub;

public class GitHubClient
(
    IHttpClientFactory httpClientFactory,
    ILogger<GitHubClient> logger
) : IGitHubClient
{
    private const string IndicatorPath = "SidekickGitHub";

    private GitHubRelease? LatestRelease { get; set; }

    public int Priority => 0;

    public Task Initialize()
    {
        return DownloadGitHubDownloadIndicatorFile();
    }

    /// <inheritdoc />
    public async Task<GitHubRelease> GetLatestRelease()
    {
        if (LatestRelease != null)
        {
            return LatestRelease;
        }

        var release = await GetLatestApiRelease();
        if (release == null || release.Tag == null)
        {
            logger.LogInformation("[Updater] No latest release found on GitHub.");
            LatestRelease = new GitHubRelease();
            return LatestRelease;
        }

        logger.LogInformation("[Updater] Found " + release.Tag + " as latest version on GitHub.");
        var latestVersion = new Version(Regex.Match(release.Tag, @"(\d+\.){2}\d+").ToString());
        var currentVersion = GetCurrentVersion();
        if (currentVersion == null)
        {
            LatestRelease = new GitHubRelease();
            return LatestRelease;
        }

        LatestRelease = new GitHubRelease
        {
            IsNewerVersion = currentVersion.CompareTo(latestVersion) < 0,
            IsExecutable = release.Assets?.Any(x => x.Name == "Sidekick-Setup.exe") ?? false,
        };
        return LatestRelease;
    }

    /// <inheritdoc />
    public async Task<bool> DownloadLatest(string downloadPath)
    {
        var release = await GetLatestApiRelease();
        if (release == null)
        {
            return false;
        }

        if (File.Exists(downloadPath))
        {
            File.Delete(downloadPath);
        }

        var downloadUrl = release.Assets?.FirstOrDefault(x => x.Name == "Sidekick-Setup.exe")?.DownloadUrl;
        if (downloadUrl == null)
        {
            return false;
        }

        using var client = GetHttpClient();
        var response = await client.GetAsync(downloadUrl);
        await using var downloadStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await downloadStream.CopyToAsync(fileStream);
        return true;
    }

    private async Task DownloadGitHubDownloadIndicatorFile()
    {
        var version = GetCurrentVersion();
        if (version == null)
        {
            logger.LogInformation("[GitHubClient] Could not get current version.");
            return;
        }

        if (HasIndicatorFile(version))
        {
            logger.LogInformation("[GitHubClient] GitHub download indicator file already exists.");
            return;
        }

        logger.LogInformation("[GitHubClient] Checking for GitHub download indicator file.");
        var apiReleases = await GetApiReleases();
        var apiRelease = apiReleases?.FirstOrDefault(x => x.Version == version);
        if (apiRelease == null)
        {
            logger.LogInformation("[GitHubClient] Could not find GitHub release for current version.");
            return;
        }

        var downloadUrl = apiRelease.Assets?.FirstOrDefault(x => x.Name == "download-instructions.txt")?.DownloadUrl;
        if (string.IsNullOrEmpty(downloadUrl))
        {
            logger.LogInformation("[GitHubClient] Could not find download indicator file for current version on GitHub.");
            return;
        }

        using var client = GetHttpClient();
        var response = await client.GetAsync(downloadUrl);
        await using var downloadStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(GetDownloadIndicatorFileName(version), FileMode.Create, FileAccess.Write, FileShare.None);
        await downloadStream.CopyToAsync(fileStream);

        logger.LogInformation("[GitHubClient] Downloaded indicator file for current version on GitHub.");
    }

    private HttpClient GetHttpClient()
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://api.github.com");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    private async Task<Release[]?> GetApiReleases()
    {
        // Get List of releases
        using var client = GetHttpClient();
        var listResponse = await client.GetAsync("/repos/Sidekick-Poe/Sidekick/releases");
        if (!listResponse.IsSuccessStatusCode)
        {
            return [];
        }

        return await JsonSerializer.DeserializeAsync<Release[]>(utf8Json: await listResponse.Content.ReadAsStreamAsync(),
                                                                options: new JsonSerializerOptions
                                                                {
                                                                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                                                    PropertyNameCaseInsensitive = true,
                                                                });
    }

    private async Task<Release?> GetLatestApiRelease()
    {
        var githubReleaseList = await GetApiReleases();
        return githubReleaseList?.FirstOrDefault(x => !x.Prerelease);
    }

    private bool HasIndicatorFile(Version version)
    {
        EnsureIndicatorDirectory();
        var fileName = GetDownloadIndicatorFileName(version);
        return File.Exists(fileName);
    }

    private static void EnsureIndicatorDirectory()
    {
        Directory.CreateDirectory(Path.Combine(path1: SidekickPaths.GetDataFilePath(), IndicatorPath));
    }

    private static string GetDownloadIndicatorFileName(Version version)
    {
        var fileName = $"{version.ToString()}.txt";
        return Path.Combine(path1: SidekickPaths.GetDataFilePath(), IndicatorPath, fileName);
    }

    private Version? GetCurrentVersion()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName()).FirstOrDefault(x => x.Name == "Sidekick")?.Version;
    }
}
