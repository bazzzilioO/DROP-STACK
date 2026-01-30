using DropStack.Core;

namespace DropStack.Modifiers
{
    public class ExtraPickModifier : IModifier
    {
        public string Id => "EXTRA_PICK";
        public string DisplayName => "Extra Pick";
        public string Description => "Next choice has 4 cards";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateExtraPick();
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ClearExtraPick();
        }
    }
}
