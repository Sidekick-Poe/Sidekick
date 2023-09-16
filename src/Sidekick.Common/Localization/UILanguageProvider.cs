using System.Globalization;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Localization
{
    /// <summary>
    /// Implementation of the ui language provider.
    /// </summary>
    public class UILanguageProvider : IUILanguageProvider
    {
        private static readonly string[] SupportedLanguages = new[] { "en", "fr", "de", "zh-tw" };
        private readonly ISettings settings;

        public UILanguageProvider(ISettings settings)
        {
            this.settings = settings;
        }

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Critical;

        /// <inheritdoc/>
        public Task Initialize()
        {
            Set(settings.Language_UI);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public List<CultureInfo> GetList()
        {
            var languages = SupportedLanguages
                .Select(x => CultureInfo.GetCultureInfo(x))
                .ToList();
            return languages;
        }

        /// <inheritdoc/>
        public void Set(string name)
        {
            var languages = GetList();
            string? language = name;
            if (!languages.Any(x => x.Name == language))
            {
                language = languages.FirstOrDefault()?.Name;
            }

            if (!string.IsNullOrEmpty(language))
            {
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(language);
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(language);
            }
        }
    }
}
