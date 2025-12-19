using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public interface IPropertyParser : IInitializableService
{
    TDefinition GetDefinition<TDefinition>() where TDefinition : PropertyDefinition;

    void Parse(Item item);

    void ParseAfterModifiers(Item item);

    Task<List<PropertyFilter>> GetFilters(Item item);
}
