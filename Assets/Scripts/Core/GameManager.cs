using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DropStack.Meta;
using DropStack.Modifiers;
using DropStack.Services;
using DropStack.UI;
using DropStack.Visual;
using UnityEngine;

namespace DropStack.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Spawner spawner;
        [SerializeField] private MergeSystem mergeSystem;
        [SerializeField] private InputController inputController;
        [SerializeField] private GameOverTrigger gameOverTrigger;
        [SerializeField] private UIController uiController;
        [SerializeField] private PixelBackground background;

        private GameConfig gameConfig;
        private SaveData saveData;
        private ModifierManager modifierManager;
        private DailyMissions dailyMissions;
        private EconomySystem economySystem;
        private UnlockSystem unlockSystem;

        private int score;
        private int maxTierReached;
        private bool isGameOver;
        private float runStartTime;
        private int nextModifierScoreThreshold;
        private int continueCount;

        public GameConfig Config => gameConfig;
        public SaveData SaveData => saveData;
        public int Score => score;
        public int MaxTierReached => maxTierReached;
        public bool IsGameOver => isGameOver;
        public ModifierManager Modifiers => modifierManager;

        public void Configure(Spawner spawnerRef, MergeSystem mergeRef, InputController inputRef, GameOverTrigger gameOverRef, UIController uiRef, PixelBackground backgroundRef)
        {
            spawner = spawnerRef;
            mergeSystem = mergeRef;
            inputController = inputRef;
            gameOverTrigger = gameOverRef;
            uiController = uiRef;
            background = backgroundRef;
        }

        private void Awake()
        {
            ServiceLocator.Initialize();
            gameConfig = ConfigLoader.Load();
            saveData = SaveSystem.Load();
            economySystem = new EconomySystem(gameConfig.economy, saveData);
            unlockSystem = new UnlockSystem(gameConfig.modifiers, saveData);
            dailyMissions = new DailyMissions(gameConfig, saveData);
            modifierManager = new ModifierManager(gameConfig, unlockSystem, this);
        }

        private void Start()
        {
            if (!ValidateSceneReferences())
            {
                return;
            }
            runStartTime = Time.time;
            score = 0;
            maxTierReached = 0;
            continueCount = 0;
            isGameOver = false;
            nextModifierScoreThreshold = gameConfig.gameplay.scoreForModifierPick;

            mergeSystem.Initialize(this, modifierManager);
            spawner.Initialize(this, modifierManager);
            inputController.Initialize(this, spawner);
            gameOverTrigger.Initialize(this);
            uiController.Initialize(this, modifierManager, dailyMissions, economySystem, unlockSystem);

            background.Initialize(gameConfig, saveData);
            uiController.UpdateScore(score, saveData.bestScore);
            uiController.UpdateNextPreview(spawner.CurrentTier);
            uiController.UpdateCurrency(saveData.currency);
            uiController.UpdateMissions(dailyMissions.GetActiveMissions());
            ServiceLocator.Analytics.RunStart();
        }

        private void Update()
        {
            modifierManager.Update(Time.deltaTime);
        }

        public void AddScore(int amount)
        {
            float multiplier = modifierManager.ScoreMultiplier;
            int finalAmount = Mathf.RoundToInt(amount * multiplier);
            score += finalAmount;
            maxTierReached = Mathf.Max(maxTierReached, spawner.LastSpawnedTier);
            uiController.UpdateScore(score, saveData.bestScore);
            dailyMissions.RecordScore(finalAmount);
            if (score >= nextModifierScoreThreshold)
            {
                nextModifierScoreThreshold += gameConfig.gameplay.scoreForModifierPick;
                StartCoroutine(ShowModifierChoiceWithDelay());
            }
        }

        public void RegisterMerge(int tier)
        {
            dailyMissions.RecordMerge();
            dailyMissions.RecordTierReached(tier);
        }

        public void HandlePieceSpawned(int tier)
        {
            uiController.UpdateNextPreview(spawner.CurrentTier);
        }

        public void OnPieceEnteredGameOverZone()
        {
            if (modifierManager.HasShield)
            {
                modifierManager.ConsumeShield();
                uiController.ShowToast("Shield used!");
                return;
            }
            if (!isGameOver)
            {
                StartCoroutine(GameOverAfterDelay());
            }
        }

        private IEnumerator GameOverAfterDelay()
        {
            yield return new WaitForSeconds(gameConfig.gameplay.gameOverDelay);
            if (gameOverTrigger.HasPieceInZone)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            if (isGameOver)
            {
                return;
            }
            isGameOver = true;
            saveData.bestScore = Mathf.Max(saveData.bestScore, score);
            saveData.runIndex += 1;
            dailyMissions.CommitMissionProgress();
            SaveSystem.Save(saveData);

            ServiceLocator.Analytics.RunEnd(score, maxTierReached, Time.time - runStartTime);
            bool canShowInterstitial = !ServiceLocator.Iap.IsNoAds() && saveData.runIndex % 3 == 0;
            if (canShowInterstitial)
            {
                ServiceLocator.Ads.ShowInterstitial();
            }
            uiController.ShowDeathScreen(score, saveData.bestScore, continueCount == 0);
        }

        public void ContinueRun()
        {
            if (continueCount > 0)
            {
                return;
            }
            continueCount += 1;
            isGameOver = false;
            gameOverTrigger.ClearZone();
            uiController.HideDeathScreen();
            uiController.ShowToast("Continue!");
        }

        public void ResetRun()
        {
            isGameOver = false;
            score = 0;
            maxTierReached = 0;
            continueCount = 0;
            nextModifierScoreThreshold = gameConfig.gameplay.scoreForModifierPick;
            mergeSystem.ClearAllPieces();
            spawner.ResetSpawner();
            uiController.UpdateScore(score, saveData.bestScore);
            uiController.UpdateNextPreview(spawner.CurrentTier);
            runStartTime = Time.time;
            ServiceLocator.Analytics.RunStart();
        }

        public void GrantCurrency(int amount)
        {
            saveData.currency += amount;
            SaveSystem.Save(saveData);
            uiController.UpdateCurrency(saveData.currency);
        }

        private IEnumerator ShowModifierChoiceWithDelay()
        {
            if (uiController.IsChoosingModifier)
            {
                yield break;
            }
            yield return new WaitForSeconds(gameConfig.gameplay.modifierPickCooldown);
            if (!isGameOver)
            {
                uiController.ShowModifierChoices(modifierManager.GetRandomChoices(uiController.ExtraPickCount));
            }
        }

        private bool ValidateSceneReferences()
        {
            bool isValid = true;
            if (spawner == null)
            {
                Debug.LogError("GameManager missing Spawner. Ensure Bootstrapper created it.");
                isValid = false;
            }
            if (mergeSystem == null)
            {
                Debug.LogError("GameManager missing MergeSystem. Ensure Bootstrapper created it.");
                isValid = false;
            }
            if (inputController == null)
            {
                Debug.LogError("GameManager missing InputController. Ensure Bootstrapper created it.");
                isValid = false;
            }
            if (gameOverTrigger == null)
            {
                Debug.LogError("GameManager missing GameOverTrigger. Ensure Bootstrapper created it.");
                isValid = false;
            }
            if (uiController == null)
            {
                Debug.LogError("GameManager missing UIController. Ensure Bootstrapper created it.");
                isValid = false;
            }
            if (background == null)
            {
                Debug.LogError("GameManager missing PixelBackground. Ensure Bootstrapper created it.");
                isValid = false;
            }
            return isValid;
        }
    }
}
