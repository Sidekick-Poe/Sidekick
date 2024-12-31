using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Properties;

public interface IPropertyParser: IInitializableService
{
    ItemProperties Parse(ParsingItem parsingItem, List<ModifierLine> modifierLines);
}
