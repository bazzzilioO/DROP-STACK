using System.Collections.Generic;
using DropStack.Core;

namespace DropStack.Meta
{
    public class CosmeticsManager
    {
        private readonly SaveData saveData;

        public CosmeticsManager(SaveData data)
        {
            saveData = data;
        }

        public IReadOnlyList<string> UnlockedPalettes => saveData.unlockedPalettes;

        public void UnlockPalette(string paletteId)
        {
            if (!saveData.unlockedPalettes.Contains(paletteId))
            {
                saveData.unlockedPalettes.Add(paletteId);
                SaveSystem.Save(saveData);
            }
        }
    }
}
