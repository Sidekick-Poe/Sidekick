using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.GitHub.Api;
using Sidekick.Apis.GitHub.Models;

namespace Sidekick.Apis.GitHub;

public class GitHubClient(
    IHttpClientFactory httpClientFactory,
    ILogger<GitHubClient> logger) : IGitHubClient
{
    private GitHubRelease? LatestRelease { get; set; }

    /// <inheritdoc />
    public async Task<GitHubRelease> GetLatestRelease()
    {
        if (LatestRelease != null)
        {
            return LatestRelease;
        }

        var release = await GetApiRelease();
        if (release == null || release.Tag == null)
        {
            logger.LogInformation("[Updater] No latest release found on GitHub.");
            LatestRelease = new GitHubRelease();
            return LatestRelease;
        }

        logger.LogInformation("[Updater] Found " + release.Tag + " as latest version on GitHub.");
        var latestVersion = new Version(
            Regex
                .Match(release.Tag, @"(\d+\.){2}\d+")
                .ToString());
        var currentVersion = AppDomain
                             .CurrentDomain.GetAssemblies()
                             .FirstOrDefault(x => x.FullName?.Contains("Sidekick") ?? false)
                             ?.GetName()
                             .Version;
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
        var release = await GetApiRelease();
        if (release == null)
        {
            return false;
        }

        if (File.Exists(downloadPath))
        {
            File.Delete(downloadPath);
        }

        var downloadUrl = release.Assets?.FirstOrDefault(x => x.Name == "Sidekick-Setup.exe")
                                 ?.DownloadUrl;
        if (downloadUrl == null)
        {
            return false;
        }

        using var client = GetHttpClient();
        var response = await client.GetAsync(downloadUrl);
        await using var downloadStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(
            downloadPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);
        await downloadStream.CopyToAsync(fileStream);
        return true;
    }

    private HttpClient GetHttpClient()
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://api.github.com");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    private async Task<Release?> GetApiRelease()
    {
        // Get List of releases
        using var client = GetHttpClient();
        var listResponse = await client.GetAsync("/repos/Sidekick-Poe/Sidekick/releases");
        if (!listResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var githubReleaseList = await JsonSerializer.DeserializeAsync<Release[]>(
            utf8Json: await listResponse.Content.ReadAsStreamAsync(),
            options: new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
            });

        if (githubReleaseList == null)
        {
            return null;
        }

        return githubReleaseList.FirstOrDefault(x => !x.Prerelease);
    }
}
