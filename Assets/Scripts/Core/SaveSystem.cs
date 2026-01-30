using UnityEngine;

namespace DropStack.Core
{
    public static class SaveSystem
    {
        private const string SaveKey = "DROP_STACK_SAVE";

        public static SaveData Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                return new SaveData();
            }
            string json = PlayerPrefs.GetString(SaveKey);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data ?? new SaveData();
        }

        public static void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }
    }
}
