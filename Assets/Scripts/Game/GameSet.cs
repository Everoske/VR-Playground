using ShootingGallery.UI;
using System.Linq;
using System;
using UnityEngine;
using ShootingGallery.Data;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    // Represents a game set or a series of rounds using a particular weapon/s
    public class GameSet : MonoBehaviour, ISaveable
    {
        [SerializeField]
        private string gameSetName;

        [SerializeField]
        private string difficulty;

        [SerializeField]
        private string id;

        [ContextMenu("Generate Guid")]
        private void GenerateGuid()
        {
            id = System.Guid.NewGuid().ToString();
        }

        [SerializeField]
        private string weaponSmallName;
        [SerializeField]
        private string weaponLargeName;

        [SerializeField]
        private GameObject weaponSmallPrefab;
        [SerializeField] 
        private GameObject weaponLargePrefab;

        [SerializeField]
        private GameObject[] ammoVolumePrefabs;

        [SerializeField]
        private GalleryRound[] rounds;

        [SerializeField]
        private float timeBeforeStart = 5.0f;

        [SerializeField]
        private float timeBetweenRounds = 5.0f;

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

        private RoundUI roundUI;

        public bool GameSetActive => gameActive;
        public int HighestPossibleScore => highestPossibleScore;

        public RoundUI RoundUI
        {
            get => roundUI;
            set => roundUI = value;
        }

        public string GetGameSetName() => gameSetName;
        public int GetNumberRounds() => rounds.Length;
        public string GetDifficulty() => difficulty;
        public string GetWeaponSmallName() => weaponSmallName;
        public string GetWeaponLargeName() => weaponLargeName;

        public GameObject[] GetAmmoVolumePrefabs() => ammoVolumePrefabs;
        public int GetHighScore() => highScore;

        public int GetHighestPossibleScore()
        {
            if (highestPossibleScore < 0)
            {
                CalculateHighestScore();
            }

            return highestPossibleScore;
        }

        public GameObject GetWeaponSmallPrefab() => weaponSmallPrefab;
        public GameObject GetWeaponLargePrefab() => weaponLargePrefab;

        public UnityAction<int> onGameSetEnd;
        public UnityAction<int> onHighScoreChanged;

        private void Update()
        {
            ProcessTimer();
        }

        private void OnEnable()
        {
            foreach (GalleryRound round in rounds)
            {
                round.onRoundReleased += RoundComplete;
            }
        }

        private void OnDisable()
        {
            foreach (GalleryRound round in rounds)
            {
                round.onRoundReleased -= RoundComplete;
            }
        }

        public void StartGameSet()
        {
            if (gameActive) return;
            if (rounds == null || rounds.Length == 0)
            {
                EndGame();
                return;
            }

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
            int finalScore = CalculateAccuracyBonus() + ScoreLocator.GetScoreTracker().CurrentScore;
            ScoreLocator.GetScoreTracker().CurrentScore = finalScore;
            onGameSetEnd?.Invoke(finalScore);

            gameActive = false;
            shouldEndGame = false;

            if (finalScore > highScore)
            {
                highScore = finalScore;
                onHighScoreChanged?.Invoke(highScore);
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
