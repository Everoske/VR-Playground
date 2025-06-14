using ShootingGallery.UI;
using System.Linq;
using System;
using UnityEngine;

namespace ShootingGallery.Game
{
    // Represents a game set or a series of rounds using a particular weapon/s
    public class GameSet : MonoBehaviour
    {
        // Weapons Used in Game
        // [SerializeField]
        // private XRWeapon[] usableWeapons;

        [SerializeField]
        private GalleryRound[] rounds;

        [SerializeField]
        private float timeBeforeStart = 5.0f;

        [SerializeField]
        private float timeBetweenRounds = 5.0f;

        [SerializeField]
        private RoundUI roundUI;

        [SerializeField]
        private int maxAccuracyBonus = 1000;

        [Range(0, 1)]
        [SerializeField]
        private float minAccuracyForBonus = 0.80f;

        private int activeRoundIndex = 0;
        private int highestPossibleScore = -1;
        private ScoreTracker scoreTracker;
        private AccuracyTracker accuracyTracker;
        private float roundTimer = 0.0f; 
        private bool timerActive = false;

        private bool gameActive = false;

        private void Awake()
        {
            scoreTracker = new ScoreTracker();
            accuracyTracker = new AccuracyTracker();
            ScoreLocator.Provide(scoreTracker);
            AccuracyLocator.Provide(accuracyTracker);
        }

        private void Update()
        {
            if (highestPossibleScore < 0)
            {
                CalculateHighestScore();
            }

            ProcessTimer();
        }

        private void OnEnable()
        {
            foreach (GalleryRound round in rounds)
            {
                round.onRoundReleased += RoundComplete;
            }

            scoreTracker.onUpdateScore += ScoreUpdated;
        }

        private void OnDisable()
        {
            foreach (GalleryRound round in rounds)
            {
                round.onRoundReleased -= RoundComplete;
            }

            scoreTracker.onUpdateScore -= ScoreUpdated;
        }

        public void StartGameSet() // Allow something that manages game sets to control whether a game set can start
        {
            if (gameActive) return;
            if (rounds == null || rounds.Length == 0)
            {
                EndGame();
                return;
            }

            Debug.Log("Starting Game Set");
            scoreTracker.ResetScore();
            accuracyTracker.ResetAccuracyTracker();
            activeRoundIndex = 0;
            gameActive = true;
            StartRoundTimer("Starting Game in:", timeBeforeStart);
        }

        public void InitiateStopGameSet()
        {
            if (!gameActive) return;

            if (timerActive)
            {
                roundTimer = 0.0f;
                timerActive = false;
                roundUI.DeactivateTimerUI();
                rounds[activeRoundIndex].FreeTargetPool();
                activeRoundIndex = rounds.Length;
                return;
            }
            
            StopCurrentRound();
        }

        private void RoundComplete()
        {
            activeRoundIndex++;
            if (activeRoundIndex >= rounds.Length)
            {
                EndGame();
            }
            else
            {
                StartRoundTimer("Time until next round:", timeBetweenRounds);
            }
        }

        private void ScoreUpdated(int score)
        {
            roundUI.SetScoreText(score);
        }

        // Calculate highest score in game set
        // Include max accuracy bonus
        private void CalculateHighestScore()
        {
            int highestRawScore = 0;
            foreach (GalleryRound round in rounds)
            {
                highestRawScore += round.GetTotalGalleryRoundScore();
            }

            highestPossibleScore = highestRawScore + maxAccuracyBonus;
        }

        // Do once at end of game
        private int CalculateAccuracyBonus()
        {
            if (accuracyTracker.GetAccuracy() >= minAccuracyForBonus)
            {
                return (int) (accuracyTracker.GetAccuracy() * maxAccuracyBonus);
            }

            return 0;
        }

        private void StartCurrentRound()
        { 
            roundUI.SetCurrentRoundText(activeRoundIndex + 1);
            rounds[activeRoundIndex].InitiateGalleryRound();
        }

        private void StopCurrentRound()
        {
            if (activeRoundIndex >= rounds.Length) return;
            rounds[activeRoundIndex].InitiateStopRound();
        }

        private void EndGame()
        {
            // Inform class controlling GameSets that game is over
            // Calculate final score/perhaps send to above class
            int finalScore = CalculateAccuracyBonus() + scoreTracker.CurrentScore;
            roundUI.SetScoreText(finalScore);
            gameActive = false;
            Debug.Log($"Accuracy: {accuracyTracker.GetAccuracy() * 100}");
            Debug.Log($"Accuracy Bonus: {finalScore - scoreTracker.CurrentScore}");
            Debug.Log("GameSet Ended");
        }

        private void StartRoundTimer(string header, float time)
        {
            roundTimer = time;
            timerActive = true;
            roundUI.SetTimerLabel(header);
            roundUI.ActivateTimerUI();
            SetupCurrentRound();
        }

        private void ProcessTimer()
        {
            if (!timerActive) return;

            roundTimer -= Time.deltaTime;

            if (roundTimer <= 0.0f)
            {
                roundUI.SetTimerText(0, 0);
                timerActive = false;
                roundUI.DeactivateTimerUI();
                StartCurrentRound();
            }
            else
            {
                int minutes = (int)roundTimer / 60;
                int seconds = (int)roundTimer % 60;
                roundUI.SetTimerText(minutes, seconds);
            }
        }

        private void SetupCurrentRound()
        {
            rounds[activeRoundIndex].AssignRoundSets();
        }
    }
}
