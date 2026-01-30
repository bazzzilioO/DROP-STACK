using System;
using System.Collections.Generic;
using UnityEngine;

namespace DropStack.Core
{
    [Serializable]
    public class GameConfig
    {
        public List<TierConfig> tiers = new List<TierConfig>();
        public List<SpawnWeight> spawnWeights = new List<SpawnWeight>();
        public List<ModifierConfig> modifiers = new List<ModifierConfig>();
        public EconomyConfig economy = new EconomyConfig();
        public GameplayConfig gameplay = new GameplayConfig();
    }

    [Serializable]
    public class TierConfig
    {
        public int tier;
        public float radius;
        public float mass;
        public int colorId;
        public int score;
    }

    [Serializable]
    public class SpawnWeight
    {
        public int tier;
        public float weight;
    }

    [Serializable]
    public class ModifierConfig
    {
        public string id;
        public string displayName;
        public string description;
        public int cost;
        public bool unlockedByDefault;
        public float value;
        public float duration;
    }

    [Serializable]
    public class EconomyConfig
    {
        public int dailyRewardCurrency = 50;
        public int missionRewardCurrency = 30;
        public int starterPackPrice = 199;
        public int noAdsPrice = 299;
        public int paletteUnlockPrice = 150;
    }

    [Serializable]
    public class GameplayConfig
    {
        public float mergeVelocityThreshold = 1.5f;
        public int scoreForModifierPick = 250;
        public float modifierPickCooldown = 3f;
        public int maxTier = 8;
        public float gameOverDelay = 1f;
    }
}
