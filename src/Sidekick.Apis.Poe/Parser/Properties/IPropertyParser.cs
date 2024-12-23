using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Properties;

public interface IPropertyParser: IInitializableService
{
    Common.Game.Items.Properties Parse(ParsingItem parsingItem, List<ModifierLine> modifierLines);
}
