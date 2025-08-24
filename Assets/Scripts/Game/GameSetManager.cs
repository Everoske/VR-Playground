using ShootingGallery.UI;
using UnityEngine;

namespace ShootingGallery.Game
{
    // Will allow for selecting, starting, and stopping different game sets
    public class GameSetManager : MonoBehaviour
    {
        [SerializeField]
        private GameSet[] gameSets;
        [SerializeField]
        private GunDrawer gunDrawer;

        [SerializeField]
        private RoundUI roundUI;

        private ScoreTracker scoreTracker;
        private AccuracyTracker accuracyTracker;

        private int selectedSet = 0;

        private void Awake()
        {
            scoreTracker = new ScoreTracker();
            accuracyTracker = new AccuracyTracker();
            ScoreLocator.Provide(scoreTracker);
            AccuracyLocator.Provide(accuracyTracker);
        }

        private void Start()
        {
            // TODO: REMOVE - FOR TESTING ONLY
            SpawnCurrentSetWeapons();

            // END TODO

            foreach (GameSet gameSet in gameSets)
            {
                gameSet.RoundUI = roundUI;
            }

            // Display the default (0) GameSet on Start
        }

        private void OnEnable()
        {
            foreach (GameSet gameSet in gameSets)
            {
                gameSet.onGameSetEnd += OnGameSetEnd;
            }

            scoreTracker.onUpdateScore += ScoreUpdated;
        }

        private void OnDisable()
        {
            foreach (GameSet gameSet in gameSets)
            {
                gameSet.onGameSetEnd -= OnGameSetEnd;
            }

            scoreTracker.onUpdateScore -= ScoreUpdated;
        }

        public void SelectGameSet(int setIndex)
        {
            if (setIndex >= gameSets.Length) return;
            selectedSet = setIndex;
            // Despawn guns first, close gun drawer
            // Wait for gun drawer to close then spawn new guns
        }

        public void SpawnCurrentSetWeapons()
        {
            if (SetCurrentlyActive()) return;

            gunDrawer.SpawnGuns(gameSets[selectedSet].GetWeaponSmallPrefab(), gameSets[selectedSet].GetWeaponLargePrefab());
        }

        public void StartSelectedSet()
        {
            if (SetCurrentlyActive()) return;

            Debug.Log("Starting Selected Game Set");
            ScoreLocator.GetScoreTracker().ResetScore();
            AccuracyLocator.GetAccuracyTracker().ResetAccuracyTracker();
            gameSets[selectedSet].StartGameSet();
        }

        public void StopActiveSet()
        {
            if (!SetCurrentlyActive()) return;

            gameSets[selectedSet].InitiateStopGameSet();
        }

        private bool SetCurrentlyActive()
        {
            return gameSets[selectedSet].GameSetActive;
        }

        private void OnGameSetEnd(int finalScore)
        {
            roundUI.SetScoreText(finalScore);
        }

        private void OnHighScoreChanged(int highScore)
        {
            // Update the high score for the selected set in the UI
        }

        private void OnDrawerClose()
        {
            // Check if there is a selected game set
            // If not, return
            // If yes, spawn guns of selected game set
        }

        private void ScoreUpdated(int score)
        {
            roundUI.SetScoreText(score);
        }
    }
}
