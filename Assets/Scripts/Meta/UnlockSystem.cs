using System.Collections.Generic;
using System.Linq;
using DropStack.Core;

namespace DropStack.Meta
{
    public class UnlockSystem
    {
        private readonly List<ModifierConfig> modifiers;
        private readonly SaveData saveData;

        public UnlockSystem(List<ModifierConfig> configs, SaveData data)
        {
            modifiers = configs;
            saveData = data;
            if (saveData.unlockedModifiers.Count == 0)
            {
                foreach (ModifierConfig modifier in modifiers)
                {
                    saveData.unlockedModifiers.Add(modifier.id);
                }
                SaveSystem.Save(saveData);
            }
        }

        public IEnumerable<string> GetUnlocked()
        {
            return saveData.unlockedModifiers.ToList();
        }

        public bool IsUnlocked(string id)
        {
            return saveData.unlockedModifiers.Contains(id);
        }

        public void Unlock(string id)
        {
            if (saveData.unlockedModifiers.Contains(id))
            {
                return;
            }
            saveData.unlockedModifiers.Add(id);
            SaveSystem.Save(saveData);
        }

        public List<string> GetUnlockedModifiers()
        {
            return saveData.unlockedModifiers.ToList();
        }

        public bool UnlockModifier(string id, int cost)
        {
            if (saveData.unlockedModifiers.Contains(id))
            {
                return false;
            }
            if (saveData.currency < cost)
            {
                return false;
            }
            saveData.currency -= cost;
            saveData.unlockedModifiers.Add(id);
            SaveSystem.Save(saveData);
            return true;
        }
    }
}
