using Microsoft.Extensions.Logging;
using Sidekick.Common.Extensions;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Game.Languages
{
    public class GameLanguageProvider : IGameLanguageProvider
    {
        private const string EnglishLanguageCode = "en";
        private readonly ILogger<GameLanguageProvider> logger;
        private readonly ISettings settings;

        public GameLanguageProvider(
            ILogger<GameLanguageProvider> logger,
            ISettings settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public IGameLanguage? Language { get; private set; }

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.High;

        /// <inheritdoc/>
        public Task Initialize()
        {
            SetLanguage(settings.Language_Parser);
            return Task.CompletedTask;
        }

        public void SetLanguage(string languageCode)
        {
            var availableLanguages = GetList();
            var language = availableLanguages.Find(x => x.LanguageCode == languageCode);

            if (language == null || language.ImplementationType == null)
            {
                logger.LogWarning("[GameLanguage] Couldn't find language matching {language}. Setting language to English.", languageCode);
                SetLanguage(EnglishLanguageCode);
                return;
            }

            Language = (IGameLanguage?)Activator.CreateInstance(language.ImplementationType);
        }

        public List<GameLanguageAttribute> GetList()
        {
            var result = new List<GameLanguageAttribute>();

            foreach (var type in typeof(GameLanguageAttribute).GetTypesImplementingAttribute())
            {
                var attribute = type.GetAttribute<GameLanguageAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                attribute.ImplementationType = type;
                result.Add(attribute);
            }

            return result;
        }

        public IGameLanguage? Get(string code)
        {
            var languages = GetList();

            var implementationType = languages.FirstOrDefault(x => x.LanguageCode == code)?.ImplementationType;
            if (implementationType != default)
            {
                return (IGameLanguage?)Activator.CreateInstance(implementationType);
            }

            return null;
        }

        public bool IsEnglish()
        {
            return Language?.LanguageCode == EnglishLanguageCode;
        }
    }
}
