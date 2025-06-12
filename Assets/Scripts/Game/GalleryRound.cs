using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ShootingGallery.Game
{
    // Represents a single round with a series of Round Sets
    public class GalleryRound : MonoBehaviour
    {
        [SerializeField]
        private RoundSet[] sets;

        private int activeRoundSetIndex = 0;
        private int roundScore = 0; // Needs rework

        public UnityAction<int> onRoundComplete;

        public void InitiateGalleryRound()
        {
            if (sets == null || sets.Length == 0)
            {
                ReleaseGalleryRound();
                return;
            }

            roundScore = 0;
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
                set.onRoundSetComplete += RoundSetComplete;
            }
        }

        private void UnsubscribeFromRoundSets()
        {
            foreach (RoundSet set in sets)
            {
                set.onRoundSetComplete -= RoundSetComplete;
            }
        }

        private void RoundSetComplete(int setScore)
        {
            roundScore += setScore;
            activeRoundSetIndex++;

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
            onRoundComplete?.Invoke(roundScore);
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
    }
}
