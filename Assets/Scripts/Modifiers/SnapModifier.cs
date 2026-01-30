using DropStack.Core;

namespace DropStack.Modifiers
{
    public class SnapModifier : IModifier
    {
        public string Id => "SNAP";
        public string DisplayName => "Snap";
        public string Description => "Auto align for merges";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateSnap(10f);
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
