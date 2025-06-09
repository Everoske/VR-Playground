using UnityEngine;

namespace ShootingGallery.Game
{
    // Represents a series of target sets to spawn
    public class RoundSet : MonoBehaviour
    {
        [SerializeField]
        private TargetSet[] targetSets;

        [SerializeField]
        private float setTime = 10.0f; // Total time to run set

        [SerializeField]
        private float timeBeforeSet = 1.0f; // Time before set

        public float TimeBeforeSet
        {
            get => timeBeforeSet;
            private set => timeBeforeSet = value;
        }

        public void InitiateRoundSet()
        {
            // TODO: Start Round Set Timer

            foreach (TargetSet targetSet in targetSets) 
            {
                targetSet.InitiateTargetSet();
            }
        }
    }
}
