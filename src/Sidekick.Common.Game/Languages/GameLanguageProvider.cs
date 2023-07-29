using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Extensions;

namespace Sidekick.Common.Game.Languages
{
    public class GameLanguageProvider : IGameLanguageProvider
    {
        private const string EnglishLanguageCode = "en";
        private readonly ILogger<GameLanguageProvider> logger;

        public GameLanguageProvider(
            ILogger<GameLanguageProvider> logger)
        {
            this.logger = logger;
        }

        public IGameLanguage Language { get; set; }

        public void SetLanguage(string languageCode)
        {
            var availableLanguages = GetList();
            var language = availableLanguages.Find(x => x.LanguageCode == languageCode);

            if (language == null)
            {
                logger.LogWarning("Couldn't find language matching {language}.", languageCode);
                return;
            }

            Language = (IGameLanguage)Activator.CreateInstance(language.ImplementationType);
        }

        public List<GameLanguageAttribute> GetList()
        {
            var result = new List<GameLanguageAttribute>();

            foreach (var type in typeof(GameLanguageAttribute).GetTypesImplementingAttribute())
            {
                var attribute = type.GetAttribute<GameLanguageAttribute>();
                attribute.ImplementationType = type;
                result.Add(attribute);
            }

            return result;
        }

        public IGameLanguage Get(string code)
        {
            var languages = GetList();

            var implementationType = languages.FirstOrDefault(x => x.LanguageCode == code)?.ImplementationType;
            if (implementationType != default)
            {
                return (IGameLanguage)Activator.CreateInstance(implementationType);
            }

            return null;
        }

        public bool IsEnglish()
        {
            return Language.LanguageCode == EnglishLanguageCode;
        }
    }
}
