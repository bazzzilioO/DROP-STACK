using DropStack.Core;

namespace DropStack.Modifiers
{
    public class ShieldModifier : IModifier
    {
        public string Id => "SHIELD";
        public string DisplayName => "Shield";
        public string Description => "Ignore one overflow";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateShield();
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
