using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Parser.ApiInformation;

public interface IApiInformationParser
{
    void Parse(Item item);
}
