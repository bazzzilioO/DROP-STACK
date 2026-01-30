using DropStack.Core;
using UnityEngine;

namespace DropStack.Modifiers
{
    public class SplitModifier : IModifier
    {
        public string Id => "SPLIT";
        public string DisplayName => "Split";
        public string Description => "Split a large tier";

        public void Activate(GameManager manager, ModifierManager modifierManager)
        {
            GamePiece target = Object.FindObjectOfType<GamePiece>();
            modifierManager.TriggerSplit(target);
        }

        public void Deactivate(GameManager manager, ModifierManager modifierManager)
        {
        }
    }
}
