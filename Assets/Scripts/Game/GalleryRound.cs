using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    // Represents a single round with a series of Round Sets
    public class GalleryRound : MonoBehaviour
    {
        [SerializeField]
        private TargetPool targetPool;

        [SerializeField]
        private RoundSet[] sets;

        private int activeRoundSetIndex = 0;

        public UnityAction onRoundReleased;

        public void InitiateGalleryRound()
        {
            if (sets == null || sets.Length == 0)
            {
                ReleaseGalleryRound();
                return;
            }

            activeRoundSetIndex = 0;
            HandleStartRoundSet();
        }

        private void OnEnable()
        {
            SubscribeToRoundSets();
        }

        private void OnDisable()
        {
            UnsubscribeFromRoundSets();
        }

        private void SubscribeToRoundSets()
        {
            foreach (RoundSet set in sets)
            {
                set.onRoundSetReleased += RoundSetComplete;
            }
        }

        private void UnsubscribeFromRoundSets()
        {
            foreach (RoundSet set in sets)
            {
                set.onRoundSetReleased -= RoundSetComplete;
            }
        }

        private void RoundSetComplete()
        {
            activeRoundSetIndex++;
            targetPool.FreePools();

            if (activeRoundSetIndex >= sets.Length)
            {
                ReleaseGalleryRound();
                return;
            }

            HandleStartRoundSet();
        }

        /// <summary>
        /// Inform GameSet that the GalleryRound has completed.
        /// </summary>
        private void ReleaseGalleryRound()
        {
            onRoundReleased?.Invoke();
        }

        private void HandleStartRoundSet()
        {
            if (sets[activeRoundSetIndex].TimeBeforeSet > 0.0f)
            {
                StartCoroutine(StartSetAfterDelay());
            }
            else
            {
                sets[activeRoundSetIndex].InitiateRoundSet();
            }
        }

        private IEnumerator StartSetAfterDelay()
        {
            yield return new WaitForSeconds(sets[activeRoundSetIndex].TimeBeforeSet);
            sets[activeRoundSetIndex].InitiateRoundSet();
        }

        /// <summary>
        /// Initiate the stop procedure so the GalleryRound can be released.
        /// </summary>
        public void InitiateStopRound()
        {
            if (activeRoundSetIndex >= sets.Length) return;
            sets[activeRoundSetIndex].InitiateStopRoundSet();
        }

        public void AssignRoundSets()
        {
            foreach (RoundSet set in sets)
            {
                set.AssignTargetSets(ref targetPool);
            }
        }

        public void FreeTargetPool()
        {
            targetPool.FreePools();
        }

        public int GetTotalGalleryRoundScore()
        {
            int score = 0;
            foreach (RoundSet set in sets)
            {
                score += set.GetTotalRoundSetScore();
            }

            return score;
        }
    }
}
