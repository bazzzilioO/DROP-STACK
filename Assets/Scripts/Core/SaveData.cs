using System;
using System.Collections.Generic;

namespace DropStack.Core
{
    [Serializable]
    public class SaveData
    {
        public int bestScore;
        public int currency;
        public List<string> unlockedModifiers = new List<string>();
        public List<string> unlockedPalettes = new List<string>();
        public DailyMissionState dailyMissionState = new DailyMissionState();
        public int runIndex;
        public long lastMissionRefreshTicks;
    }
}
