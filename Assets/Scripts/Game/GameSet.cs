using ShootingGallery.UI;
using System.Linq;
using System;
using UnityEngine;
using ShootingGallery.Data;

namespace ShootingGallery.Game
{
    // Represents a game set or a series of rounds using a particular weapon/s
    public class GameSet : MonoBehaviour, ISaveable
    {
        [SerializeField]
        private string gameSetName;

        [SerializeField]
        private string id;

        [ContextMenu("Generate Guid")]
        private void GenerateGuid()
        {
            id = System.Guid.NewGuid().ToString();
        }

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

        private int highScore = 0;
        private int activeRoundIndex = 0;
        private int highestPossibleScore = -1;
        private float roundTimer = 0.0f; 
        private bool timerActive = false;
        private bool shouldEndGame = false;

        private bool gameActive = false;

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

            ScoreLocator.GetScoreTracker().onUpdateScore += ScoreUpdated;
        }

        private void OnDisable()
        {
            foreach (GalleryRound round in rounds)
            {
                round.onRoundReleased -= RoundComplete;
            }

            ScoreLocator.GetScoreTracker().onUpdateScore -= ScoreUpdated;
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
            ScoreLocator.GetScoreTracker().ResetScore();
            AccuracyLocator.GetAccuracyTracker().ResetAccuracyTracker();
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
            }

            shouldEndGame = true;
            StopCurrentRound();
        }

        private void RoundComplete()
        {
            Debug.Log("Round Complete");
            activeRoundIndex++;
            if (activeRoundIndex >= rounds.Length || shouldEndGame)
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
            if (activeRoundIndex < rounds.Length) return 0; // Return 0 if game ended early

            if (AccuracyLocator.GetAccuracyTracker().GetAccuracy() >= minAccuracyForBonus)
            {
                return (int)(AccuracyLocator.GetAccuracyTracker().GetAccuracy() * maxAccuracyBonus);
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
            int finalScore = CalculateAccuracyBonus() + ScoreLocator.GetScoreTracker().CurrentScore;
            roundUI.SetScoreText(finalScore);
            gameActive = false;
            shouldEndGame = false;
            Debug.Log($"Accuracy: {AccuracyLocator.GetAccuracyTracker().GetAccuracy() * 100}");
            Debug.Log($"Accuracy Bonus: {finalScore - ScoreLocator.GetScoreTracker().CurrentScore}");
            Debug.Log("GameSet Ended");

            if (finalScore > highScore)
            {
                highScore = finalScore;
            }
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

        public void LoadData(GameData data)
        {
            if (data.highScoreData.ContainsKey(id))
            {
                highScore = data.highScoreData[id];
            }
        }

        public void SaveData(ref GameData data)
        {
            if (data.highScoreData.ContainsKey(id))
            {
                data.highScoreData.Remove(id);
            }
            data.highScoreData.Add(id, highScore);
        }
    }
}
