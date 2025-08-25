using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShootingGallery.UI
{
    public class GameSelectUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject setSelectionUI;

        [SerializeField]
        private TMP_Text selectedSetNameText;
        [SerializeField]
        private TMP_Text setInfoNameText;
        [SerializeField]
        private TMP_Text numberRoundsText;
        [SerializeField]
        private TMP_Text difficultyText;
        [SerializeField]
        private TMP_Text weapon1Text;
        [SerializeField]
        private TMP_Text weapon2Text;
        [SerializeField]
        private TMP_Text highestPossibleText;
        [SerializeField]
        private TMP_Text bestScoreText;

        [SerializeField]
        private Button selectButton;
        [SerializeField]
        private Button rightButton;
        [SerializeField]
        private Button leftButton;

        public void ShowGameSelectionUI()
        {
            setSelectionUI.SetActive(true);
        }

        public void HideGameSelectionUI()
        {
            setSelectionUI.SetActive(false);
        }

        public void SetSelectedSetNameText(string name)
        {
            selectedSetNameText.text = name;
        }

        public void SetInfoNameText(string name)
        {
            setInfoNameText.text = name;
        }

        public void SetNumberRoundsText(int numberRounds)
        {
            numberRoundsText.text = numberRounds.ToString();
        }

        public void SetDifficultyText(string difficulty)
        {
            difficultyText.text = difficulty;
        }

        public void SetWeapon1Text(string weapon1)
        {
            weapon1Text.text = weapon1;
        }

        public void SetWeapon2Text(string weapon2)
        {
            weapon2Text.text = weapon2;
        }

        public void SetHighestPossibleText(int highestPossible)
        {
            highestPossibleText.text = highestPossible.ToString();
        }

        public void SetBestScoreText(int bestScore)
        {
            bestScoreText.text = bestScore.ToString();
        }

        public void ToggleSelectButtonInteraction(bool interactable)
        {
            selectButton.interactable = interactable;
        }

        public void ToggleRightButtonInteraction(bool interactable)
        {
            rightButton.interactable = interactable;
        }

        public void ToggleLeftButtonInteraction(bool interactable)
        {
            leftButton.interactable = interactable;
        }
    }
}
