using UnityEngine;
using UnityEngine.Events;

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

        private int totalPointsEarned = 0;
        private int targetSetsComplete = 0;

        public UnityAction<int> onRoundSetComplete; // Inform GalleryRound with total points

        private void OnEnable()
        {
            SubscribeToTargetSets();
        }

        private void OnDisable()
        {
            UnsubscribeFromTargetSets();
        }

        private void ShootingTargetHit(int points)
        {
            totalPointsEarned = Mathf.Max(totalPointsEarned + points, 0);
        }

        private void TargetSetComplete()
        {
            targetSetsComplete++;
            if (targetSetsComplete >= targetSets.Length)
            {
                onRoundSetComplete?.Invoke(totalPointsEarned);
            }
        }

        // Stop any target set currently active
        // Target sets should call onSetComplete when they are out of view from player
        private void StopTargetSets()
        {
            foreach (TargetSet targetSet in targetSets)
            {
                //targetSet.StopTargetSet();
            }
        }

        private void SubscribeToTargetSets()
        {
            foreach (TargetSet targetSet in targetSets)
            {
                targetSet.onTargetHit += ShootingTargetHit;
                targetSet.onTargetSetComplete += TargetSetComplete;
            }
        }

        private void UnsubscribeFromTargetSets()
        {
            foreach (TargetSet targetSet in targetSets)
            {
                targetSet.onTargetHit -= ShootingTargetHit;
                targetSet.onTargetSetComplete -= TargetSetComplete;
            }
        }

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
