using DropStack.Core;

namespace DropStack.Modifiers
{
    public class IceModifier : IModifier
    {
        public string Id => "ICE";
        public string DisplayName => "Ice";
        public string Description => "Stabilize the cup";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateIce(10f);
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
