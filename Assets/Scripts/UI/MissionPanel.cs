using System.Collections.Generic;
using DropStack.Meta;
using TMPro;
using UnityEngine;

namespace DropStack.UI
{
    public class MissionPanel : MonoBehaviour
    {
        [SerializeField] private List<TMP_Text> missionTexts = new List<TMP_Text>();

        public void SetMissions(IReadOnlyList<DailyMissionProgress> missions)
        {
            for (int i = 0; i < missionTexts.Count; i++)
            {
                if (i >= missions.Count)
                {
                    missionTexts[i].text = string.Empty;
                    continue;
                }
                DailyMissionProgress mission = missions[i];
                missionTexts[i].text = $"{mission.id} {mission.current}/{mission.target}";
            }
        }
    }
}
