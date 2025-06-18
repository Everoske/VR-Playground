using UnityEngine;

namespace ShootingGallery.Game
{
    public class AccuracyTracker 
    {
        private int shotsFired = 0;
        private int targetsHit = 0;

        public void ResetAccuracyTracker()
        {
            shotsFired = 0;
            targetsHit = 0;
        }

        public void IncrementShotsFired()
        {
            shotsFired++;
        }

        public void IncrementTargetsHit()
        {
            targetsHit++;
        }

        public float GetAccuracy()
        {
            if (shotsFired == 0) return 0.0f;
            return ((float) targetsHit / (float) shotsFired);
        }
    }
}
