using TMPro;
using UnityEngine;

namespace ShootingGallery.UI
{
    public class RoundUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreText;
        [SerializeField]
        private TMP_Text roundText;
        [SerializeField]
        private GameObject timerUI;
        [SerializeField]
        private TMP_Text timerText;

        public void ActivateTimerUI()
        {
            timerUI.SetActive(true);
        }

        public void DeactivateTimerUI()
        {
            timerUI.SetActive(false);
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
    }
}

