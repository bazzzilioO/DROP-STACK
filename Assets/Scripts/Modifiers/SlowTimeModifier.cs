using DropStack.Core;

namespace DropStack.Modifiers
{
    public class SlowTimeModifier : IModifier
    {
        public string Id => "SLOW_TIME";
        public string DisplayName => "Slow Time";
        public string Description => "Slow physics";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateSlowTime(5f);
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
