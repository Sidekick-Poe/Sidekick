using Sidekick.Apis.Poe.Languages;
namespace Sidekick.Data.Fuzzy;

public interface IFuzzyService
{
    string CleanFuzzyText(IGameLanguage language, string text);
}
