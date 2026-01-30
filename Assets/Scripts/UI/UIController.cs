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

        public bool IsChoosingModifier => cardChoiceView.IsVisible;
        public int ExtraPickCount => modifierManager.ExtraPickActive ? 1 : 0;

        public void Initialize(GameManager manager, ModifierManager modifiers, DailyMissions missions, EconomySystem economy, UnlockSystem unlocks)
        {
            gameManager = manager;
            modifierManager = modifiers;
            dailyMissions = missions;
            economySystem = economy;
            unlockSystem = unlocks;

            cardChoiceView.Initialize(this);
            deathScreenView.Initialize(this);
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
            cardChoiceView.Show(cards);
        }

        public void PickModifier(string id)
        {
            modifierManager.ActivateModifier(id);
            cardChoiceView.Hide();
            modifierManager.ClearExtraPick();
        }

        public void ShowDeathScreen(int score, int best, bool canContinue)
        {
            deathScreenView.Show(score, best, canContinue);
        }

        public void HideDeathScreen()
        {
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
            if (!ServiceLocator.Ads.CanShowRewarded(RewardedType.Continue))
            {
                return;
            }
            ServiceLocator.Ads.ShowRewarded(RewardedType.Continue);
            gameManager.ContinueRun();
        }

        public void OnRetryButton()
        {
            gameManager.ResetRun();
        }

        public void OnRerollButton()
        {
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
