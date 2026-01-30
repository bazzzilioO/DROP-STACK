using DropStack.Core;

namespace DropStack.Modifiers
{
    public interface IModifier
    {
        string Id { get; }
        string DisplayName { get; }
        string Description { get; }
        void Activate(GameManager manager, ModifierManager modifierManager);
        void Deactivate(GameManager manager, ModifierManager modifierManager);
    }
}
