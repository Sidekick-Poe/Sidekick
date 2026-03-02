using Sidekick.Data.Languages;
namespace Sidekick.Data.Fuzzy;

public interface IFuzzyService
{
    string CleanFuzzyText(IGameLanguage language, string text);
}
