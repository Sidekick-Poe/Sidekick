using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.GitHub.Api;
using Sidekick.Apis.GitHub.Models;

namespace Sidekick.Apis.GitHub;

public class GitHubClient
(
    IHttpClientFactory httpClientFactory,
    ILogger<GitHubClient> logger
) : IGitHubClient
{
    private DateTimeOffset? LastUpdateCheck { get; set; }
    private GitHubRelease? LatestRelease { get; set; }
    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <inheritdoc />
    public async Task<GitHubRelease> GetLatestRelease()
    {
        if (LatestRelease != null && LastUpdateCheck >= DateTimeOffset.Now.AddHours(-6))
        {
            return LatestRelease;
        }

        LastUpdateCheck = DateTimeOffset.Now;

        if (Debugger.IsAttached)
        {
            return new GitHubRelease()
            {
                IsNewerVersion = false,
                IsExecutable = false,
            };
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
        try
        {
            using var client = GetHttpClient();
            var listResponse = await client.GetAsync("/repos/Sidekick-Poe/Sidekick/releases");
            if (!listResponse.IsSuccessStatusCode)
            {
                return [];
            }

            var content = await listResponse.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Release[]>(content, JsonSerializerOptions);
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "[GitHubClient] Error while trying to get releases from GitHub.");
            return [];
        }
    }

    private async Task<Release?> GetLatestApiRelease()
    {
        var githubReleaseList = await GetApiReleases();
        return githubReleaseList?.FirstOrDefault(x => !x.Prerelease);
    }

    private static Version? GetCurrentVersion()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName()).FirstOrDefault(x => x.Name == "Sidekick")?.Version;
    }
}
