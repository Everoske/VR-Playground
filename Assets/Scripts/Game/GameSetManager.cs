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
    }
}
