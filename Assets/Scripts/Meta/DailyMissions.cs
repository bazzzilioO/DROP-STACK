using System;
using System.Collections.Generic;
using DropStack.Core;
using UnityEngine;

namespace DropStack.Meta
{
    [Serializable]
    public class DailyMissionState
    {
        public List<DailyMissionProgress> missions = new List<DailyMissionProgress>();
    }

    [Serializable]
    public class DailyMissionProgress
    {
        public string id;
        public int current;
        public int target;
        public bool completed;
    }

    public class DailyMissions
    {
        private readonly GameConfig config;
        private readonly SaveData saveData;
        private readonly List<DailyMissionProgress> activeMissions = new List<DailyMissionProgress>();

        public DailyMissions(GameConfig gameConfig, SaveData data)
        {
            config = gameConfig;
            saveData = data;
            RefreshIfNeeded();
        }

        public IReadOnlyList<DailyMissionProgress> GetActiveMissions()
        {
            return activeMissions;
        }

        public void RecordMerge()
        {
            UpdateMission("merge", 1);
        }

        public void RecordTierReached(int tier)
        {
            UpdateMission("tier", tier);
        }

        public void RecordScore(int score)
        {
            UpdateMission("score", score);
        }

        public void CommitMissionProgress()
        {
            saveData.dailyMissionState.missions = new List<DailyMissionProgress>(activeMissions);
            SaveSystem.Save(saveData);
        }

        private void RefreshIfNeeded()
        {
            DateTime now = DateTime.UtcNow.Date;
            DateTime last = new DateTime(saveData.lastMissionRefreshTicks, DateTimeKind.Utc).Date;
            if (saveData.dailyMissionState.missions.Count == 0 || now > last)
            {
                saveData.lastMissionRefreshTicks = DateTime.UtcNow.Ticks;
                activeMissions.Clear();
                activeMissions.Add(new DailyMissionProgress { id = "merge", target = 10, current = 0 });
                activeMissions.Add(new DailyMissionProgress { id = "tier", target = 5, current = 0 });
                activeMissions.Add(new DailyMissionProgress { id = "score", target = 500, current = 0 });
                saveData.dailyMissionState.missions = new List<DailyMissionProgress>(activeMissions);
            }
            else
            {
                activeMissions.AddRange(saveData.dailyMissionState.missions);
            }
        }

        private void UpdateMission(string id, int value)
        {
            foreach (DailyMissionProgress mission in activeMissions)
            {
                if (mission.id != id || mission.completed)
                {
                    continue;
                }
                if (id == "tier")
                {
                    mission.current = Mathf.Max(mission.current, value);
                }
                else
                {
                    mission.current += value;
                }
                if (mission.current >= mission.target)
                {
                    mission.completed = true;
                }
            }
        }
    }
}
