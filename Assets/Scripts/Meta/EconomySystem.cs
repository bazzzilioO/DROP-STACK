using DropStack.Core;

namespace DropStack.Meta
{
    public class EconomySystem
    {
        private readonly EconomyConfig config;
        private readonly SaveData saveData;

        public EconomySystem(EconomyConfig economyConfig, SaveData data)
        {
            config = economyConfig;
            saveData = data;
        }

        public int Currency => saveData.currency;

        public bool Spend(int amount)
        {
            if (saveData.currency < amount)
            {
                return false;
            }
            saveData.currency -= amount;
            SaveSystem.Save(saveData);
            return true;
        }

        public void GrantMissionReward()
        {
            saveData.currency += config.missionRewardCurrency;
            SaveSystem.Save(saveData);
        }
    }
}
