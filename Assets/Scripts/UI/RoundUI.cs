using TMPro;
using UnityEngine;

namespace ShootingGallery.UI
{
    /// <summary>
    /// Class used for displaying information about rounds during an
    /// active GameSet.
    /// </summary>
    public class RoundUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text roundText;
        [SerializeField]
        private TMP_Text scoreText;
        [SerializeField]
        private TMP_Text accuracyText;
        [SerializeField]
        private GameObject timerUI;
        [SerializeField]
        private TMP_Text timerHeaderText;
        [SerializeField]
        private TMP_Text timerText;
        [SerializeField]
        private GameObject finalScoreUI;
        [SerializeField]
        private TMP_Text finalScoreText;

        public void ActivateTimerUI()
        {
            timerUI.SetActive(true);
        }

        public void DeactivateTimerUI()
        {
            timerUI.SetActive(false);
        }

        public void ShowFinalScoreUI()
        {
            finalScoreUI.SetActive(true);
        }

        public void HideFinalScoreUI()
        {
            finalScoreUI.SetActive(false);
        }

        public void SetTimerLabel(string header)
        {
            timerHeaderText.text = header;
        }

        public void SetTimerText(int minutes, int seconds)
        {
            timerText.text = $"{minutes:D2}:{seconds:D2}";
        }

        public void SetCurrentRoundText(int currentRound)
        {
            roundText.text = currentRound.ToString();
        }

        public void SetScoreText(int score)
        {
            scoreText.text = score.ToString();
        }

        public void SetAccuracyText(float accuracy)
        {
            accuracyText.text = accuracy.ToString();
        }

        public void SetFinalScoreText(int score)
        {
            finalScoreText.text = score.ToString();
        }
    }
}

