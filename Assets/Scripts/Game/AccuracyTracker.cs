using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    public class AccuracyTracker 
    {
        private int shotsFired = 0;
        private int targetsHit = 0;
        private bool trackingPaused = true;

        public UnityAction onShotFired;

        public void ResetAccuracyTracker()
        {
            shotsFired = 0;
            targetsHit = 0;
        }

        public void IncrementShotsFired()
        {
            if (trackingPaused) return;
            shotsFired++;
            onShotFired?.Invoke();
        }

        public void IncrementTargetsHit()
        {
            if (trackingPaused) return;
            targetsHit++;
        }

        public float GetAccuracy()
        {
            if (shotsFired == 0) return 0.0f;
            return ((float) targetsHit / (float) shotsFired);
        }

        public void PauseTracking()
        {
            trackingPaused = true;
        }

        public void UnpauseTracking()
        {
            trackingPaused = false;
        }
    }
}
