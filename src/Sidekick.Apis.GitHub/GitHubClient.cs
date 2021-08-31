using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.GitHub.Models;
using Sidekick.Common;

namespace Sidekick.Apis.GitHub
{
    public class GitHubClient : IGitHubClient
    {
        private readonly ILogger<GitHubClient> logger;
        private readonly HttpClient client;

        public GitHubClient(
            IHttpClientFactory httpClientFactory,
            ILogger<GitHubClient> logger)
        {
            this.logger = logger;
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Determines latest release on github. Pre-releases do not count as release, therefore we need to get the list of releases first, if no actual latest release can be found
        /// </summary>
        /// <returns></returns>
        public async Task<GitHubRelease> GetLatestRelease()
        {
            // Get List of releases
            var listResponse = await client.GetAsync("/repos/Sidekick-Poe/Sidekick/releases");
            if (listResponse.IsSuccessStatusCode)
            {
                var githubReleaseList = await JsonSerializer.DeserializeAsync<GitHubRelease[]>(await listResponse.Content.ReadAsStreamAsync(), new JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });
                return githubReleaseList.FirstOrDefault(x => !x.Prerelease);
            }

            return null;
        }

        /// <summary>
        /// Determines if there is a newer version available
        /// </summary>
        /// <returns></returns>
        public bool IsUpdateAvailable(GitHubRelease release)
        {
            if (release != null)
            {
                logger.LogInformation("[Updater] Found " + release.Tag + " as latest version on GitHub.");

                var latestVersion = new Version(Regex.Match(release.Tag, @"(\d+\.){2}\d+").ToString());
                var currentVersion = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Contains("Sidekick")).GetName().Version;

                var result = currentVersion.CompareTo(latestVersion);
                return result < 0;
            }
            else
            {
                logger.LogInformation("[Updater] No latest release found on GitHub.");
            }

            return false;
        }

        /// <summary>
        /// Downloads the latest release from github
        /// </summary>
        /// <returns></returns>
        public async Task<string> DownloadRelease(GitHubRelease release)
        {
            if (release == null) return null;

            var downloadPath = SidekickPaths.GetDataFilePath("Sidekick-Update.exe");
            if (File.Exists(downloadPath)) File.Delete(downloadPath);

            var downloadUrl = release.Assets.FirstOrDefault(x => x.Name == "Sidekick-Setup.exe")?.DownloadUrl;
            if (downloadUrl == null) return null;

            var response = await client.GetAsync(downloadUrl);
            using var downloadStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await downloadStream.CopyToAsync(fileStream);

            return downloadPath;
        }
    }
}
