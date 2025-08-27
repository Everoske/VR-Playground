using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    public class ScoreTracker 
    {
        private int currentScore;

        public int CurrentScore
        {
            get => currentScore;
            set
            {
                currentScore = Mathf.Max(value, 0);
            }
        }

        public UnityAction<int> onUpdateScore;

        public void ResetScore()
        {
            currentScore = 0;
            onUpdateScore?.Invoke(currentScore);
        }

        public void AddToScore(int score)
        {
            currentScore = Mathf.Max(currentScore + score, 0);
            onUpdateScore?.Invoke(currentScore);
        }
    }
}
