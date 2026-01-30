using DropStack.Core;

namespace DropStack.Modifiers
{
    public class CleanseModifier : IModifier
    {
        public string Id => "CLEANSE";
        public string DisplayName => "Cleanse";
        public string Description => "Remove smallest tier";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.TriggerCleanse();
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
