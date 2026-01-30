using System.Collections.Generic;
using DropStack.Modifiers;
using UnityEngine;
using UnityEngine.UI;

namespace DropStack.UI
{
    public class CardChoiceView : MonoBehaviour
    {
        [SerializeField] private List<Button> cardButtons = new List<Button>();
        [SerializeField] private List<Image> cardIcons = new List<Image>();
        [SerializeField] private List<Text> cardTitles = new List<Text>();
        [SerializeField] private List<Text> cardDescriptions = new List<Text>();

        private UIController uiController;

        public bool IsVisible => gameObject.activeSelf;

        public void Initialize(UIController controller)
        {
            uiController = controller;
            gameObject.SetActive(false);
        }

        public void Show(List<ModifierCardData> cards)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < cardButtons.Count; i++)
            {
                if (i >= cards.Count)
                {
                    cardButtons[i].gameObject.SetActive(false);
                    continue;
                }
                ModifierCardData data = cards[i];
                cardButtons[i].gameObject.SetActive(true);
                int index = i;
                cardButtons[i].onClick.RemoveAllListeners();
                cardButtons[i].onClick.AddListener(() => uiController.PickModifier(cards[index].id));
                if (i < cardIcons.Count)
                {
                    cardIcons[i].sprite = data.icon;
                }
                if (i < cardTitles.Count)
                {
                    cardTitles[i].text = data.displayName;
                }
                if (i < cardDescriptions.Count)
                {
                    cardDescriptions[i].text = data.description;
                }
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
