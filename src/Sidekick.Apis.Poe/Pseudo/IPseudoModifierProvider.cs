using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Apis.Poe.Pseudo
{
    public interface IPseudoModifierProvider
    {
        Task Initialize();

        List<Modifier> Parse(List<ModifierLine> modifiers);
    }
}
