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
        private TargetPool targetPool;

        [SerializeField]
        private GalleryRound[] rounds;

        [SerializeField]
        private float timeBetweenRounds = 5.0f;

        [SerializeField]
        private ScoreTracker scoreTracker;

        [SerializeField]
        private RoundUI roundUI;

        [SerializeField]
        private int maxAccuracyBonus = 1000;

        [Range(0, 1)]
        [SerializeField]
        private float minAccuracyForBonus = 0.80f;

        private int activeRoundIndex = 0;
        private int highestPossibleScore = 0;
        private float accuracy = 0.0f; // How accurate player is
        private float roundTimer = 0.0f; 
        private bool timerActive = false;

        private bool gameActive = false;

        private void Start()
        {
            CalculateHighestScore();
        }

        private void Update()
        {
            ProcessTimer();
        }

        private void OnEnable()
        {
            foreach (GalleryRound round in rounds)
            {
                round.onRoundComplete += RoundComplete;
            }
        }

        private void OnDisable()
        {
            foreach (GalleryRound round in rounds)
            {
                round.onRoundComplete -= RoundComplete;
            }
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
            activeRoundIndex = 0;
            gameActive = true;
            //StartCurrentRound();
            StartRoundTimer();
        }

        public void InitiateStopGameSet()
        {
            if (!gameActive) return;
            StopCurrentRound();
        }

        private void RoundComplete(int score)
        {
            activeRoundIndex++;
            if (!gameActive || activeRoundIndex >= rounds.Length)
            {
                EndGame();
            }
            else
            {
                StartRoundTimer();
            }
        }

        // Calculate highest score in game set
        // Include max accuracy bonus
        private void CalculateHighestScore()
        {

        }

        // Do once at end of game
        private int CalculateAccuracyBonus()
        {
            throw new System.NotImplementedException();
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
            roundTimer = 0.0f;
            timerActive = false;
            roundUI.DeactivateTimerUI();
            // Inform class controlling GameSets that game is over
            // Calculate final score/perhaps send to above class
            //int finalScore = CalculateAccuracyBonus() + scoreTracker.CurrentScore;
            gameActive = false;
            Debug.Log("GameSet Ended");
        }

        // StartRoundTimer(float time)
        private void StartRoundTimer()
        {
            roundTimer = timeBetweenRounds;
            timerActive = true;
            roundUI.ActivateTimerUI();
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
    }
}
