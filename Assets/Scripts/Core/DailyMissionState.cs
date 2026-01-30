using System;
using System.Collections.Generic;
using UnityEngine;
using DropStack.Meta;

[Serializable]
public class DailyMissionState
{
    public string dayKey;

    // ВАЖНО: используем родной тип проекта
    public List<DailyMissionProgress> missions = new List<DailyMissionProgress>();

    public int runsToday;
}
