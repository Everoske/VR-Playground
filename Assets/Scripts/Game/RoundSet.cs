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

        private int totalTargetSetsComplete = 0;
        private int totalNonDecoyOnlySets = 0;
        private int nonDecoyOnlySetsComplete = 0;

        public UnityAction onRoundSetReleased;

        private void Start()
        {
            DetermineNumberOfNonDecoyOnlySets();
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
        private void TargetSetReleased(bool isDecoySet)
        {
            totalTargetSetsComplete++;
            if (totalTargetSetsComplete >= targetSets.Length)
            {
                ReleaseRoundSet();
                return;
            }

            if (isDecoySet) return;

            nonDecoyOnlySetsComplete++;
            if (nonDecoyOnlySetsComplete >= totalNonDecoyOnlySets)
            {
                InitiateStopRoundSet();
            } 
        }

        /// <summary>
        /// Inform GalleryRound that RoundSet has been terminated.
        /// </summary>
        private void ReleaseRoundSet()
        {
            ResetRoundSet();
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
            StopTargetSets(); // Perhaps add a check here to see if round already stopped?
        }

        /// <summary>
        /// Determine number of non-decoy only target sets.
        /// </summary>
        private void DetermineNumberOfNonDecoyOnlySets()
        {
            foreach (TargetSet targetSet in targetSets)
            {
                if (!targetSet.IsDecoyOnly())
                {
                    totalNonDecoyOnlySets++;
                }
            }
        }

        /// <summary>
        /// Reset round set parameters.
        /// </summary>
        private void ResetRoundSet()
        {
            totalTargetSetsComplete = 0;
            nonDecoyOnlySetsComplete = 0;
        }

        /// <summary>
        /// Initiates target sets and the round set timer.
        /// </summary>
        public void InitiateRoundSet()
        {
            if (targetSets == null ||  targetSets.Length == 0 || totalNonDecoyOnlySets == 0)
            {
                StopTargetSets();
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

        /// <summary>
        /// Set all target sets to the Ready state.
        /// </summary>
        public void AssignTargetSets()
        {
            foreach (TargetSet targetSet in targetSets)
            {
                targetSet.AssignTargets();
            }
        }

        /// <summary>
        /// Retrieve total score for round set.
        /// </summary>
        /// <returns>Total round set score.</returns>
        public int GetTotalRoundSetScore()
        {
            int score = 0;
            foreach (TargetSet targetSet in targetSets)
            {
                score += targetSet.GetTotalTargetSetScore();
            }

            return score;
        }
    }
}
