using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Trade.Parser.ApiInformation;

public interface IApiInformationParser
{
    void Parse(Item item);
}
