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
        private float timeBeforeSet = 0.0f; // Time before set

        private int targetSetsComplete = 0;

        public UnityAction onRoundSetReleased; // Inform GalleryRound with total points

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
        /// Called when a target set finishes its stop sequence. Ends round set
        /// once all targets have completed their stop sequence.
        /// </summary>
        private void TargetSetReleased()
        {
            targetSetsComplete++;
            if (targetSetsComplete >= targetSets.Length)
            {
                ReleaseRoundSet();
            }
        }

        /// <summary>
        /// Inform GalleryRound that RoundSet has been terminated.
        /// </summary>
        private void ReleaseRoundSet()
        {
            onRoundSetReleased?.Invoke();
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
                targetSet.onTargetSetComplete += TargetSetReleased;
            }
        }

        /// <summary>
        /// Unsubscribes from target sets.
        /// </summary>
        private void UnsubscribeFromTargetSets()
        {
            foreach (TargetSet targetSet in targetSets)
            {
                targetSet.onTargetSetComplete -= TargetSetReleased;
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
            if (targetSets == null ||  targetSets.Length == 0)
            {
                ReleaseRoundSet();
                return;
            }

            foreach (TargetSet targetSet in targetSets) 
            {
                targetSet.InitiateTargetSet();
            }

            StartCoroutine(InitiateRoundSetTimer());
        }

        /// <summary>
        /// Initiate the stopping procedure so the RoundSet can be released.
        /// </summary>
        public void InitiateStopRoundSet()
        {
            StopTargetSets();
        }

        public void AssignTargetSets(ref TargetPool pool)
        {
            foreach (TargetSet targetSet in targetSets)
            {
                targetSet.AssignTargets(ref pool);
            }
        }
    }
}
