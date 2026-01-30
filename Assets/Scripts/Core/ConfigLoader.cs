using UnityEngine;

namespace DropStack.Core
{
    public static class ConfigLoader
    {
        public static GameConfig Load()
        {
            TextAsset asset = Resources.Load<TextAsset>("Config/game_config");
            if (asset == null)
            {
                Debug.LogWarning("Config not found, using defaults");
                return new GameConfig();
            }
            return JsonUtility.FromJson<GameConfig>(asset.text);
        }
    }
}
