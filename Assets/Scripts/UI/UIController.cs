using System.Collections.Generic;
using DropStack.Core;
using DropStack.Meta;
using DropStack.Modifiers;
using DropStack.Services;
using TMPro;
using UnityEngine;

namespace DropStack.UI
{
    public class UIController : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text bestText;
        [SerializeField] private TMP_Text currencyText;
        [SerializeField] private TMP_Text toastText;

        [Header("Views")]
        [SerializeField] private CardChoiceView cardChoiceView;
        [SerializeField] private DeathScreenView deathScreenView;
        [SerializeField] private MissionPanel missionPanel;

        private GameManager gameManager;
        private ModifierManager modifierManager;
        private DailyMissions dailyMissions;
        private EconomySystem economySystem;
        private UnlockSystem unlockSystem;

        public bool IsChoosingModifier => cardChoiceView != null && cardChoiceView.IsVisible;
        public int ExtraPickCount => modifierManager != null && modifierManager.ExtraPickActive ? 1 : 0;

        public void Configure(TMP_Text score, TMP_Text best, TMP_Text currency, TMP_Text toast, CardChoiceView cardChoice, DeathScreenView deathView, MissionPanel missions)
        {
            scoreText = score;
            bestText = best;
            currencyText = currency;
            toastText = toast;
            cardChoiceView = cardChoice;
            deathScreenView = deathView;
            missionPanel = missions;
        }

        public void Initialize(GameManager manager, ModifierManager modifiers, DailyMissions missions, EconomySystem economy, UnlockSystem unlocks)
        {
            gameManager = manager;
            modifierManager = modifiers;
            dailyMissions = missions;
            economySystem = economy;
            unlockSystem = unlocks;

            if (cardChoiceView != null)
            {
                cardChoiceView.Initialize(this);
            }
            else
            {
                Debug.LogError("UIController missing CardChoiceView. Modifier choices will be disabled.");
            }
            if (deathScreenView != null)
            {
                deathScreenView.Initialize(this);
            }
            else
            {
                Debug.LogError("UIController missing DeathScreenView. Death screen will be disabled.");
            }
        }

        public void UpdateScore(int score, int best)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score {score}";
            }
            if (bestText != null)
            {
                bestText.text = $"Best {best}";
            }
        }

        public void UpdateCurrency(int currency)
        {
            if (currencyText != null)
            {
                currencyText.text = $"Coins {currency}";
            }
        }

        public void UpdateNextPreview(int tier)
        {
        }

        public void UpdateMissions(IReadOnlyList<DailyMissionProgress> missions)
        {
            if (missionPanel != null)
            {
                missionPanel.SetMissions(missions);
            }
        }

        public void ShowModifierChoices(List<ModifierCardData> cards)
        {
            if (cardChoiceView == null)
            {
                Debug.LogError("UIController cannot show modifier choices because CardChoiceView is missing.");
                return;
            }
            cardChoiceView.Show(cards);
        }

        public void PickModifier(string id)
        {
            if (modifierManager == null)
            {
                Debug.LogError("UIController cannot pick modifier because ModifierManager is missing.");
                return;
            }
            modifierManager.ActivateModifier(id);
            if (cardChoiceView != null)
            {
                cardChoiceView.Hide();
            }
            modifierManager.ClearExtraPick();
        }

        public void ShowDeathScreen(int score, int best, bool canContinue)
        {
            if (deathScreenView == null)
            {
                Debug.LogError("UIController cannot show death screen because DeathScreenView is missing.");
                return;
            }
            deathScreenView.Show(score, best, canContinue);
        }

        public void HideDeathScreen()
        {
            if (deathScreenView == null)
            {
                return;
            }
            deathScreenView.Hide();
        }

        public void ShowToast(string message)
        {
            if (toastText != null)
            {
                toastText.text = message;
                CancelInvoke(nameof(ClearToast));
                Invoke(nameof(ClearToast), 2f);
            }
        }

        private void ClearToast()
        {
            if (toastText != null)
            {
                toastText.text = string.Empty;
            }
        }

        public void OnContinueButton()
        {
            if (gameManager == null)
            {
                Debug.LogError("UIController cannot continue because GameManager is missing.");
                return;
            }
            if (!ServiceLocator.Ads.CanShowRewarded(RewardedType.Continue))
            {
                return;
            }
            ServiceLocator.Ads.ShowRewarded(RewardedType.Continue);
            gameManager.ContinueRun();
        }

        public void OnRetryButton()
        {
            if (gameManager == null)
            {
                Debug.LogError("UIController cannot retry because GameManager is missing.");
                return;
            }
            gameManager.ResetRun();
        }

        public void OnRerollButton()
        {
            if (modifierManager == null)
            {
                Debug.LogError("UIController cannot reroll because ModifierManager is missing.");
                return;
            }
            if (!ServiceLocator.Ads.CanShowRewarded(RewardedType.Reroll))
            {
                ShowToast("Reroll cooldown");
                return;
            }
            ServiceLocator.Ads.ShowRewarded(RewardedType.Reroll);
            ShowModifierChoices(modifierManager.GetRandomChoices(ExtraPickCount));
        }
    }
}
