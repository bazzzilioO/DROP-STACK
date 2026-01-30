using DropStack.Core;

namespace DropStack.Modifiers
{
    public class DoubleMergeModifier : IModifier
    {
        public string Id => "DOUBLE_MERGE";
        public string DisplayName => "Double Merge";
        public string Description => "Next merge +2 tiers";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateDoubleMerge();
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
