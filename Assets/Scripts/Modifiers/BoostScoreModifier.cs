using DropStack.Core;

namespace DropStack.Modifiers
{
    public class BoostScoreModifier : IModifier
    {
        public string Id => "BOOST_SCORE";
        public string DisplayName => "Score Boost";
        public string Description => "+25% score";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            modifierManager.ActivateScoreBoost(20f);
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
