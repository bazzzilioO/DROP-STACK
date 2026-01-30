using DropStack.Core;

namespace DropStack.Modifiers
{
    public class MagnetModifier : IModifier
    {
        public string Id => "MAGNET";
        public string DisplayName => "Magnet";
        public string Description => "Attracts same tiers";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateMagnet(8f, 12f);
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
