namespace Sidekick.Common.Game.Languages
{
    public class GameLanguageAttribute : Attribute
    {
        public GameLanguageAttribute(string name, string languageCode)
        {
            Name = name;
            LanguageCode = languageCode;
        }

        public string Name { get; private set; }

        public string LanguageCode { get; private set; }

        public Type? ImplementationType { get; set; }
    }
}
