using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.GitHub.Api;

namespace Sidekick.Apis.GitHub
{
    public class GitHubClient : IGitHubClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<GitHubClient> logger;

        public GitHubClient(
            IHttpClientFactory httpClientFactory,
            ILogger<GitHubClient> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        private bool? UpdateAvailable { get; set; } = null;

        private HttpClient GetHttpClient()
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        /// <inheritdoc/>
        public async Task<bool> IsUpdateAvailable()
        {
            if (UpdateAvailable != null)
            {
                return UpdateAvailable.Value;
            }

            var release = await GetLatestRelease();
            if (release == null || release.Tag == null)
            {
                logger.LogInformation("[Updater] No latest release found on GitHub.");
                UpdateAvailable = false;
                return false;
            }

            logger.LogInformation("[Updater] Found " + release.Tag + " as latest version on GitHub.");
            var latestVersion = new Version(Regex.Match(release.Tag, @"(\d+\.){2}\d+").ToString());
            var currentVersion = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName?.Contains("Sidekick") ?? false)?.GetName().Version;
            if (currentVersion == null)
            {
                return false;
            }

            var result = currentVersion.CompareTo(latestVersion);
            UpdateAvailable = result < 0;

            return UpdateAvailable ?? false;
        }

        /// <inheritdoc/>
        public async Task<bool> DownloadLatest(string downloadPath)
        {
            var release = await GetLatestRelease();
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
            using var downloadStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await downloadStream.CopyToAsync(fileStream);
            return true;
        }

        private async Task<GitHubRelease?> GetLatestRelease()
        {
            // Get List of releases
            using var client = GetHttpClient();
            var listResponse = await client.GetAsync("/repos/Sidekick-Poe/Sidekick/releases");
            if (!listResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var githubReleaseList = await JsonSerializer.DeserializeAsync<GitHubRelease[]>(await listResponse.Content.ReadAsStreamAsync(), new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true
            });

            if (githubReleaseList == null)
            {
                return null;
            }

            return githubReleaseList.FirstOrDefault(x => !x.Prerelease);
        }
    }
}
