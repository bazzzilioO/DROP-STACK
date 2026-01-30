using DropStack.Core;

namespace DropStack.Modifiers
{
    public class GhostModifier : IModifier
    {
        public string Id => "GHOST";
        public string DisplayName => "Ghost";
        public string Description => "Next drop phases";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateGhost(2f);
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
