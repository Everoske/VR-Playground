using UnityEngine;

namespace ShootingGallery.Game
{
    // Will allow for selecting, starting, and stopping different game sets
    public class GameSetManager : MonoBehaviour
    {
        private ScoreTracker scoreTracker;
        private AccuracyTracker accuracyTracker;

        private void Awake()
        {
            scoreTracker = new ScoreTracker();
            accuracyTracker = new AccuracyTracker();
            ScoreLocator.Provide(scoreTracker);
            AccuracyLocator.Provide(accuracyTracker);
        }
    }
}
