using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DropStack.UI
{
    public class DeathScreenView : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text bestText;
        [SerializeField] private Button continueButton;

        private UIController uiController;

        public void Initialize(UIController controller)
        {
            uiController = controller;
            gameObject.SetActive(false);
        }

        public void Show(int score, int best, bool canContinue)
        {
            gameObject.SetActive(true);
            if (scoreText != null)
            {
                scoreText.text = $"Score {score}";
            }
            if (bestText != null)
            {
                bestText.text = $"Best {best}";
            }
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(canContinue);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
