namespace Sidekick.Apis.GitHub.Models
{
    public record GitHubRelease
    {
        public bool IsNewerVersion { get; set; }

        public bool IsExecutable { get; set; }
    }
}
