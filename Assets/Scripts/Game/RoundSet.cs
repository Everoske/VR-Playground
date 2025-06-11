using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    /// <summary>
    /// Represents a list of target sets to shoot during one sequence of a round.
    /// </summary>
    public class RoundSet : MonoBehaviour
    {
        [SerializeField]
        private TargetSet[] targetSets;

        [Tooltip("Max time for the round set to play.")]
        [SerializeField]
        private float roundSetTimer = 10.0f; // Total time to run set

        [SerializeField]
        private float timeBeforeSet = 1.0f; // Time before set

        private int totalPointsEarned = 0;
        private int targetSetsComplete = 0;

        public UnityAction<int> onRoundSetComplete; // Inform GalleryRound with total points

        public float TimeBeforeSet
        {
            get => timeBeforeSet;
            private set => timeBeforeSet = value;
        }

        private void OnEnable()
        {
            SubscribeToTargetSets();
        }

        private void OnDisable()
        {
            UnsubscribeFromTargetSets();
        }

        /// <summary>
        /// Increments the total points earned by the player for this round set.
        /// </summary>
        /// <param name="points">Points earned.</param>
        private void ShootingTargetHit(int points)
        {
            totalPointsEarned = Mathf.Max(totalPointsEarned + points, 0);
        }

        /// <summary>
        /// Called when a target set finishes its stop sequence. Ends round set
        /// once all targets have completed their stop sequence.
        /// </summary>
        private void TargetSetComplete()
        {
            targetSetsComplete++;
            if (targetSetsComplete >= targetSets.Length)
            {
                onRoundSetComplete?.Invoke(totalPointsEarned);
            }
        }

        /// <summary>
        /// Instructs all target sets to run their stop sequence.
        /// </summary>
        private void StopTargetSets()
        {
            foreach (TargetSet targetSet in targetSets)
            {
                targetSet.StopTargetSet();
            }
        }

        /// <summary>
        /// Subscribes to target sets.
        /// </summary>
        private void SubscribeToTargetSets()
        {
            foreach (TargetSet targetSet in targetSets)
            {
                targetSet.onTargetHit += ShootingTargetHit;
                targetSet.onTargetSetComplete += TargetSetComplete;
            }
        }

        /// <summary>
        /// Unsubscribes from target sets.
        /// </summary>
        private void UnsubscribeFromTargetSets()
        {
            foreach (TargetSet targetSet in targetSets)
            {
                targetSet.onTargetHit -= ShootingTargetHit;
                targetSet.onTargetSetComplete -= TargetSetComplete;
            }
        }

        /// <summary>
        /// Runs a timer for the round set and stops the round set once completed.
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitiateRoundSetTimer()
        {
            yield return new WaitForSeconds(roundSetTimer);
            StopTargetSets();
        }
        
        /// <summary>
        /// Initiates target sets and the round set timer.
        /// </summary>
        public void InitiateRoundSet()
        {
            foreach (TargetSet targetSet in targetSets) 
            {
                targetSet.InitiateTargetSet();
            }

            StartCoroutine(InitiateRoundSetTimer());
        }
    }
}
