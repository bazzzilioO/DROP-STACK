using DropStack.Core;

namespace DropStack.Modifiers
{
    public class BounceModifier : IModifier
    {
        public string Id => "BOUNCE";
        public string DisplayName => "Bounce";
        public string Description => "Extra bouncy";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateBounce(10f);
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
